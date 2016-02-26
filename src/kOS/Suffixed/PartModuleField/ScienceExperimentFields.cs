﻿using System;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using System.Linq;
using System.Reflection;
<<<<<<< HEAD
=======
using System.Collections.Generic;
>>>>>>> pull-review/1454
using kOS.Safe.Encapsulation;

namespace kOS.Suffixed.PartModuleField
{
    public abstract class ScienceExperimentFields : PartModuleFields
    {
        protected IScienceDataContainer container;
        public ScienceExperimentFields(PartModule module, SharedObjects shared) : base(module, shared)
        {
            this.container = module as IScienceDataContainer;

            if (container == null)
            {
                throw new KOSException("This module is not a science data container");
            }

            InitializeSuffixes();
        }

        private void InitializeSuffixes()
        {
            AddSuffix("DEPLOY", new NoArgsVoidSuffix(DeployExperiment, "Deploy and run this experiment"));
            AddSuffix("RESET", new NoArgsVoidSuffix(ResetExperiment, "Reset this experiment"));
            AddSuffix("TRANSMIT", new NoArgsVoidSuffix(TransmitData, "Transmit experiment data back to Kerbin"));
            AddSuffix("DUMP", new NoArgsVoidSuffix(DumpData, "Dump experiment data"));
<<<<<<< HEAD
            AddSuffix("INOPERABLE", new Suffix<BooleanValue>(() => module.Inoperable, "Is this experiment inoperable"));
            AddSuffix("DEPLOYED", new Suffix<BooleanValue>(() => module.Deployed, "Is this experiment deployed"));
            AddSuffix("RERUNNABLE", new Suffix<BooleanValue>(() => module.rerunnable, "Is this experiment rerunnable"));
            AddSuffix("HASDATA", new Suffix<BooleanValue>(() => module.GetData().Any(), "Does this experiment have any data stored"));
=======
            AddSuffix("INOPERABLE", new Suffix<BooleanValue>(() => Inoperable(), "Is this experiment inoperable"));
            AddSuffix("DEPLOYED", new Suffix<BooleanValue>(() => Deployed(), "Is this experiment deployed"));
            AddSuffix("RERUNNABLE", new Suffix<BooleanValue>(() => Rerunnable(), "Is this experiment rerunnable"));
            AddSuffix("HASDATA", new Suffix<BooleanValue>(() => HasData(), "Does this experiment have any data stored"));
            AddSuffix("DATA", new Suffix<ListValue>(Data, "Does this experiment have any data stored"));
>>>>>>> pull-review/1454
        }

        public abstract bool Deployed();
        public abstract bool Inoperable();
        public abstract void DeployExperiment();
        public abstract void ResetExperiment();

        public virtual bool Rerunnable()
        {
            return container.IsRerunnable();
        }

        public virtual bool HasData()
        {
            return container.GetData().Any();
        }

        public virtual ListValue Data()
        {
            return new ListValue(container.GetData().Select(s => new ScienceDataValue(s)).Cast<Structure>());
        }

        public virtual void DumpData()
        {
            ThrowIfNotCPUVessel();

            Array.ForEach(container.GetData(), (d) => container.DumpData(d));
        }

        public abstract void TransmitData();

        public new string ToString()
        {
            return "SCIENCE EXPERIMENT";
        }
    }
}

