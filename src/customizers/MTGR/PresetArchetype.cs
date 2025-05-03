using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer.customizers.MTGR
{
    internal class PresetArchetype
    {
        public string DescriptiveName;
        public string[] PresetModules;

        public PresetArchetype(string DescName, string[] PresetMods)
        {
            DescriptiveName = DescName;
            PresetModules = PresetMods;
        }

        public static PresetArchetype FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            return new PresetArchetype(csvparts[0], csvparts.Skip(1).ToArray());
        }
    }
}
