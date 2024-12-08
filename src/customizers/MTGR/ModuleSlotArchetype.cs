using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer.customizers.MTGR
{
    internal class ModuleSlotArchetype
    {
        public string DescriptiveName;
        public string ModuleFolder;
        public string LabelPrefix;
        public string IdPrefix;

        public ModuleSlotArchetype(string DescName, string Folder, string Label, string Id)
        {
            DescriptiveName = DescName;
            ModuleFolder = Folder;
            LabelPrefix = Label;
            IdPrefix = Id;
        }

        public static ModuleSlotArchetype FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            return new ModuleSlotArchetype(csvparts[0], csvparts[1], csvparts[2], csvparts[3]);
        }
    }
}
