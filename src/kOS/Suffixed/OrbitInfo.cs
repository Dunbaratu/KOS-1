﻿using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using System;
using kOS.Serialization;
using kOS.Safe.Serialization;
using kOS.Safe;
using kOS.Utilities;
using UnityEngine;

namespace kOS.Suffixed
{
    [kOS.Safe.Utilities.KOSNomenclature("Orbit")]
    public class OrbitInfo : Structure, IHasSharedObjects
    {
        private Orbit orbit;
        public SharedObjects Shared { get; set; }
        private string name;

        public OrbitInfo()
        {
            InitializeSuffixes();
        }

        public OrbitInfo(Orbitable orb, SharedObjects sharedObj) : this()
        {
            orbit = orb.Orbit;
            Shared = sharedObj;
            name = orb.GetName();
        }

        public OrbitInfo( Orbit orb, SharedObjects sharedObj) : this()
        {
            Shared = sharedObj;
            orbit = orb;
            name = "<unnamed>";
        }

        /// <summary>
        /// Constructs an OrbitInfo given an initial position/velocity pair of state vectors.
        /// </summary>
        /// <param name="pos">Position in current ship-raw coords (which are ship-centered)</param>
        /// <param name="vel">Velocity in current ship-raw coords (which are body-centered)</param>
        /// <param name="body">Body the orbit goes around</param>
        /// <param name="when">Universal timestamp that the pos/vel state is taken from</param>
        /// <param name="sharedObj"></param>
        public OrbitInfo(Vector pos, Vector vel, BodyTarget body, double when, SharedObjects sharedObj) : this()
        {
            Shared = sharedObj;
            orbit = new Orbit();
            // FIXME: there is a bug here in translating between ship-local and body-centered coordinate systems, if you
            // are below the body.inverseRotationAltitude when you try to compute an orbit.  you may be off by a rotation.
            // uncertain of the exact incantations to fix.  workaround is to get yourself into orbit first before trying
            // to compute orbits from this function.  mechjeb also has this bug, but nobody has reported it or fixed it.
            // It should mostly affect people who are doing orbital computation testing while on the launchpad (sometimes).
            orbit.UpdateFromStateVectors(Utils.SwapYZ(pos - body.GetPosition()), Utils.SwapYZ(vel), body.Body, when);
            if (double.IsNaN(orbit.LAN))
            {
                // not sure if this matters, but avoids annoying NaN issues in KOS with perfectly equatorial orbits
                orbit.LAN = 0;
            }
            // fix from MJ for perfectly circular orbits (not sure if any of the precision here matters)
            if (double.IsNaN(orbit.argumentOfPeriapsis))
            {
                Vector3d vectorToAN = Quaternion.AngleAxis(-(float)orbit.LAN, Planetarium.up) * Planetarium.right;
                Vector3d vectorToPe = Utils.SwapYZ(orbit.eccVec);

                double cosArgumentOfPeriapsis = Vector3d.Dot(vectorToAN, vectorToPe) / (vectorToAN.magnitude * vectorToPe.magnitude);
                if(cosArgumentOfPeriapsis > 1) {
                    orbit.argumentOfPeriapsis = 0;
                } else if (cosArgumentOfPeriapsis < -1) {
                    orbit.argumentOfPeriapsis = 180;
                } else {
                    orbit.argumentOfPeriapsis = Math.Acos(cosArgumentOfPeriapsis) * 180 / Math.PI;;
                }
            }
            name = "<user defined>";
        }
        
        /// <summary>
        /// Construct a new OrbitInfo based on the elliptical parameters of
        /// the orbit.
        /// </summary>
        /// <param name="sharedObj"></param>
        /// <param name="body">body being orbited</param>
        /// <param name="inc">inclination</param>
        /// <param name="ecc">eccentricity</param>
        /// <param name="sma">semi-major axis</param>
        /// <param name="lan">longitude of the ascending node</param>
        /// <param name="argPe">argument of periapsis</param>
        /// <param name="meanAnoEpoch">mean anomaly at epoch</param>
        /// <param name="epochUT">universal timestamp of epoch</param>
        public OrbitInfo(SharedObjects sharedObj,
                         BodyTarget body,
                         ScalarDoubleValue inc,
                         ScalarDoubleValue ecc,
                         ScalarDoubleValue sma,
                         ScalarDoubleValue lan,
                         ScalarDoubleValue argPe,
                         ScalarDoubleValue meanAnoEpoch,
                         ScalarDoubleValue epochUT )
        {
           Shared = sharedObj;
           orbit = new Orbit(inc, ecc, sma, lan, argPe, meanAnoEpoch, epochUT, body.Body);
           name = "<user defined>";
        }

        private void InitializeSuffixes()
        {
            AddSuffix("NAME", new SetSuffix<StringValue>(() => name, value => name = value.ToString()));
            AddSuffix("APOAPSIS", new Suffix<ScalarValue>(() => orbit.ApA));
            AddSuffix("PERIAPSIS", new Suffix<ScalarValue>(() => orbit.PeA));
            AddSuffix("BODY", new Suffix<BodyTarget>(() => new BodyTarget(orbit.referenceBody, Shared)));
            AddSuffix("PERIOD", new Suffix<ScalarValue>(() => orbit.period));
            AddSuffix("INCLINATION", new Suffix<ScalarValue>(() => orbit.inclination));
            AddSuffix("ECCENTRICITY", new Suffix<ScalarValue>(() => orbit.eccentricity));
            AddSuffix("SEMIMAJORAXIS", new Suffix<ScalarValue>(() => orbit.semiMajorAxis));
            AddSuffix("SEMIMINORAXIS", new Suffix<ScalarValue>(() => orbit.semiMinorAxis));
            AddSuffix(new[]{"LAN", "LONGITUDEOFASCENDINGNODE"}, new Suffix<ScalarValue>(() => orbit.LAN));
            AddSuffix("ARGUMENTOFPERIAPSIS", new Suffix<ScalarValue>(() => orbit.argumentOfPeriapsis));
            AddSuffix("TRUEANOMALY", new Suffix<ScalarValue>(() => Utilities.Utils.DegreeFix(Utilities.Utils.RadiansToDegrees(orbit.trueAnomaly),0.0)));
            AddSuffix("MEANANOMALYATEPOCH", new Suffix<ScalarValue>(() => Utilities.Utils.DegreeFix(Utilities.Utils.RadiansToDegrees(orbit.meanAnomalyAtEpoch), 0.0)));
            AddSuffix("TRANSITION", new Suffix<StringValue>(() => orbit.patchEndTransition.ToString()));
            AddSuffix("POSITION", new Suffix<Vector>(() => GetPositionAtUT( new TimeSpan(Planetarium.GetUniversalTime() ) )));
            AddSuffix("POSITIONAT", new OneArgsSuffix<Vector,ScalarValue>(ut => GetPositionAtUT( new TimeSpan(ut))));
            AddSuffix("VELOCITY", new Suffix<OrbitableVelocity>(() => GetVelocityAtUT( new TimeSpan(Planetarium.GetUniversalTime() ) )));
            AddSuffix("VELOCITYAT", new OneArgsSuffix<OrbitableVelocity,ScalarValue>(ut => GetVelocityAtUT( new TimeSpan(ut))));
            AddSuffix("NEXTPATCH", new Suffix<OrbitInfo>(GetNextPatch));
            AddSuffix("HASNEXTPATCH", new Suffix<BooleanValue>(GetHasNextPatch));
            AddSuffix("NEXTPATCHETA", new Suffix<ScalarValue>(GetNextPatchETA));

            //TODO: Determine if these vectors are different than POSITION and VELOCITY
            AddSuffix("VSTATEVECTOR", new Suffix<Vector>(() => new Vector(orbit.vel)));
            AddSuffix("RSTATEVECTOR", new Suffix<Vector>(() => new Vector(orbit.pos)));

        }

        /// <summary>
        ///   Get the position of this thing in this orbit at the given
        ///   time.  Note that it does NOT take into account any
        ///   encounters or maneuver nodes - it assumes the current
        ///   orbit patch remains followed forever.
        /// </summary>
        /// <param name="timeStamp">The universal time to query for</param>
        /// <returns></returns>
        public Vector GetPositionAtUT( TimeSpan timeStamp )
        {
            return new Vector( orbit.getPositionAtUT( timeStamp.ToUnixStyleTime() ) - Shared.Vessel.findWorldCenterOfMass() );
        }

        /// <summary>
        ///   Get the velocity pairing of this thing in this orbit at the given
        ///   time.  Note that it does NOT take into account any
        ///   encounters or maneuver nodes - it assumes the current
        ///   orbit patch remains followed forever.
        /// </summary>
        /// <param name="timeStamp">The universal time to query for</param>
        /// <returns></returns>
        public OrbitableVelocity GetVelocityAtUT( TimeSpan timeStamp )
        {
            var orbVel = new Vector( orbit.getOrbitalVelocityAtUT( timeStamp.ToUnixStyleTime() ) );
            // For some weird reason orbit returns velocities with Y and Z swapped, so flip them back:
            orbVel = new Vector( orbVel.X, orbVel.Z, orbVel.Y );
            CelestialBody parent = orbit.referenceBody;
            Vector surfVel;
            if (parent != null)
            {
                Vector3d pos = GetPositionAtUT( timeStamp );
                surfVel = new Vector( orbVel - parent.getRFrmVel( pos + Shared.Vessel.findWorldCenterOfMass()) );
            }
            else
                surfVel = new Vector( orbVel.X, orbVel.Y, orbVel.Z );
            return new OrbitableVelocity( orbVel, surfVel );
        }

        /// <summary>
        /// Return the next OrbitInfo after this one (i.e. transitional encounter)
        /// </summary>
        /// <returns>an OrbitInfo, or a null if there isn't any.</returns>
        private OrbitInfo GetNextPatch()
        {
            return ! GetHasNextPatch() ? null : new OrbitInfo(orbit.nextPatch,Shared);
        }

        /// <summary>
        /// Returns the ETA of when the nextpatch will happen
        /// </summary>
        /// <returns>A double representing the ETA in seconds, or a zero if there isn't any.</returns>
        private ScalarValue GetNextPatchETA()
        {
            if (GetHasNextPatch())
            {
                return orbit.EndUT - Planetarium.GetUniversalTime();
            }
            throw new Safe.Exceptions.KOSSituationallyInvalidException("Cannot get eta to next patch when no additional patches exist.  Try checking the HASNEXTPATCH suffix.");
        }

        /// <summary>
        /// Find out whether or not the orbit has a next patch.
        /// </summary>
        /// <returns>true if the :NEXTPATCH suffix will return a real suffix.</returns>
        private BooleanValue GetHasNextPatch()
        {
            return orbit.nextPatch != null && (orbit.nextPatch.activePatch);
        }

        public override string ToString()
        {
            return "ORBIT of " + name;
        }
    }
}
