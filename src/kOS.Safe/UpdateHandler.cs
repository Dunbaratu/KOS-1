using System.Collections.Generic;
using System.Linq;
using System;

namespace kOS.Safe
{
    public class UpdateHandler
    {
        // Using a Dictionary instead of List to prevent duplications.  If an object tries to
        // insert itself more than once into the observer list, it still only gets in the list
        // once and therefore only gets its Update() called once per update.
        // The value of the KeyValuePair, the int, is unused.
        private readonly HashSet<WeakReference> observers = new HashSet<WeakReference>();
        private readonly HashSet<WeakReference> fixedObservers = new HashSet<WeakReference>();

        public double CurrentFixedTime { get; private set; }
        public double LastDeltaFixedTime { get; private set; }
        public double CurrentTime { get; private set; }
        public double LastDeltaTime { get; private set; }

        public void AddObserver(IUpdateObserver observer)
        {
            observers.Add(new WeakReference(observer, false));
        }

        public void AddFixedObserver(IFixedUpdateObserver observer)
        {
            fixedObservers.Add(new WeakReference(observer, false));
        }

        /// <summary>
        /// To be called when a part of the KOS system wants to be removed from
        /// the updater (this is the closing call to be paired with AddObserver()).
        /// Note that calling this explicitly is not strictly required, as
        /// UpdateHandler will call this for you when the object is being disposed of.
        /// But calling it explicitly may remove it from the updater faster than
        /// relying on orphaning to do it for you.
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver(IUpdateObserver observer)
        {
            // Have to make a new weak reference just so the `Remove` method
            // has something it can run the Equals check on:
            observers.Remove(new WeakReference(observer, true));
        }

        /// To be called when a part of the KOS system wants to be removed from
        /// the FixedUpdater (this is the closing call to be paired with AddFixedObserver()).
        /// Note that calling this explicitly is not strictly required, as
        /// UpdateHandler will call this for you when the object is being disposed of.
        /// But calling it explicitly may remove it from the updater faster than
        /// relying on orphaning to do it for you.
        public void RemoveFixedObserver(IFixedUpdateObserver observer)
        {
            // Have to make a new weak reference just so the `Remove` method
            // has something it can run the Equals check on:
            fixedObservers.Remove(new WeakReference(observer, true));
        }

        public void UpdateObservers(double deltaTime)
        {
            LastDeltaTime = deltaTime;
            CurrentTime += deltaTime;
            
            var snapshot = new HashSet<WeakReference>(observers);
            foreach (var observer in snapshot)
            {
                if (observer.IsAlive && observer.Target != null)
                    ((IUpdateObserver)observer.Target).KOSUpdate(deltaTime);
                else
                    observers.Remove(observer);
            }
        }

        public void UpdateFixedObservers(double deltaTime)
        {
            LastDeltaFixedTime = deltaTime;
            CurrentFixedTime += deltaTime;
            
            var snapshot = new HashSet<WeakReference>(fixedObservers);
            foreach (var observer in snapshot)
            {
                if (observer.IsAlive && observer.Target != null)
                    ((IFixedUpdateObserver)observer.Target).KOSFixedUpdate(deltaTime);
                else
                    fixedObservers.Remove(observer);
            }
        }
        
        /// <summary>
        /// Return all the registered fixed update handlers of a particular type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IEnumerable<IFixedUpdateObserver> GetAllFixedUpdatersOfType(Type t)
        {
            IEnumerable<IFixedUpdateObserver> refs = observers.Select<WeakReference, IFixedUpdateObserver>((wref) => ((IFixedUpdateObserver)wref.Target));
            return refs.Where(item => t.IsAssignableFrom(item.GetType()));
        }
        
        /// <summary>
        /// Return all the registered update handlers of a particular type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IEnumerable<IUpdateObserver> GetAllUpdatersOfType(Type t)
        {
            IEnumerable<IUpdateObserver> refs = observers.Select<WeakReference, IUpdateObserver>((wref) => ((IUpdateObserver)wref.Target));
            return refs.Where(item => t.IsAssignableFrom(item.GetType()));
        }

    }
}
