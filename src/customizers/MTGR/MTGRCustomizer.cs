using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer.customizers.MTGR
{
    internal class MTGRCustomizer
    {
        private readonly Module BaseModule;
        private readonly List<Module> ButtonModules = new List<Module>();
        private readonly List<ModuleArchetype> ModuleArchetypes = new List<ModuleArchetype>();
        private readonly List<ModuleSlotArchetype> SlotArchetypes = new List<ModuleSlotArchetype>();
        public MTGRCustomizer(ModuleArchetype Archetype) {
            var Arch = Archetype.Clone();
            Arch.ButtonFileName = "MTGR_StandardButtons";
            BaseModule = Arch.CreateModule("Customizers\\MTGR\\");
            PopulateModuleSlots();
        }
        public void Interactive()
        {
            foreach (var slot in SlotArchetypes)
            {
                PopulateButtonModules(slot);
                int listlength = ModuleArchetypes.Count;
                Console.WriteLine("Select the button module in the {0} slot of your {1}:", slot.DescriptiveName, BaseModule.DescriptiveName);
                for (int i = 0; i < listlength; i++)
                {
                    Console.WriteLine("{0}) {1}", i + 1, ModuleArchetypes[i].DescriptiveName);
                }
                int selection = Program.PromptNumber(String.Format("{0} Module", slot.DescriptiveName), 1, listlength);
                AddButtonModule(ModuleArchetypes[selection-1], slot);
            }
        }
        private void PopulateModuleSlots()
        {
            string ModulesFilePath = string.Format("Customizers\\MTGR\\ModuleSlots.csv");
            if (File.Exists(ModulesFilePath))
            {
                StreamReader sr = File.OpenText(ModulesFilePath);
                string Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    SlotArchetypes.Add(ModuleSlotArchetype.FromCsv(Line));
                }
                sr.Close();
            }
        }
        private void PopulateButtonModules(ModuleSlotArchetype slotArchetype)
        {
            if (ModuleArchetypes.Count > 0)
            {
                ModuleArchetypes.Clear();
            }
            string ModulesFilePath = string.Format("Customizers\\MTGR\\{0}\\Modules.csv",slotArchetype.ModuleFolder);
            if (File.Exists(ModulesFilePath))
            {
                StreamReader sr = File.OpenText(ModulesFilePath);
                string Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    var Archetype = ModuleArchetype.FromCsv(Line);
                    Archetype.LabelPrefix = slotArchetype.LabelPrefix;
                    Archetype.IdPrefix = slotArchetype.IdPrefix;
                    ModuleArchetypes.Add(Archetype);
                }
                sr.Close();
            }
        }
        public void AddButtonModule(ModuleArchetype arch, ModuleSlotArchetype slot)
        {
            ButtonModules.Add(arch.CreateModule(String.Format("Customizers\\MTGR\\{0}\\",slot.ModuleFolder)));
        }
        public Module GenerateModule()
        {
            foreach (var buttonModule in ButtonModules)
            {
                buttonModule.BakeInto(BaseModule);
            }
            return BaseModule;
        }

    }
}
