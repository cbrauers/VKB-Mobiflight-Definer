using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class ModuleArchetype : SubDeviceArchetype
    {
        public string LabelPrefix;
        public string IdPrefix;
        private int ModulesCreated = 0;

        public ModuleArchetype(string DescName, string Label, string Id, string ButtonFile, string LedFile) : base(DescName, ButtonFile, LedFile)
        {
            LabelPrefix = Label;
            IdPrefix = Id;
        }

        public static ModuleArchetype FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            return new ModuleArchetype(csvparts[0], csvparts[1], csvparts[2], csvparts[3], csvparts[4]);
        }
        public Module CreateModule()
        {
            ModulesCreated++;
            string Label = LabelPrefix;
            string Id = IdPrefix;
            if (ModulesCreated > 1)
            {
                Label += String.Format(" {0}", ModulesCreated);
                Id += ModulesCreated;
            }
            return new Module(DescriptiveName, Label, Id, ButtonFileName, LedFileName);
        }
    }
}
