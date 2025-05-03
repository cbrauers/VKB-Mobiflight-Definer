using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        private readonly List<PresetArchetype> PresetArchetypes = new List<PresetArchetype>();
        public MTGRCustomizer(ModuleArchetype Archetype) {
            var Arch = Archetype.Clone();
            Arch.ButtonFileName = "MTGR_StandardButtons";
            BaseModule = Arch.CreateModule("Customizers\\MTGR\\");
            PopulateModuleSlots();
            PopulatePresets();
        }
        public void Interactive()
        {
            Console.WriteLine("Select a preset for your {0}. Enter a leading zero to allow customizing the preset:", BaseModule.DescriptiveName);
            Console.WriteLine("0) Custom, no preset");
            int listlength = PresetArchetypes.Count;
            int numslots = SlotArchetypes.Count;
            bool custom = false;
            PresetArchetype chosenPreset = null;
            for (int i = 0; i < listlength; i++)
            {
                Console.WriteLine("{0}) {1}", i + 1, PresetArchetypes[i].DescriptiveName);
            }
            int selection = Program.PromptNumber("Preset", 0, listlength, -1, out int leadingzeroes);
            if (selection > 0)
            {
                chosenPreset = PresetArchetypes[selection - 1];
            }
            if (leadingzeroes > 0 || selection == 0)
            {
                custom = true;
            }
            for (int j = 0; j < numslots; j++)
            {
                selection = -1;
                var slot = SlotArchetypes[j];
                string preset = null;
                if (chosenPreset != null)
                {
                    preset = chosenPreset.PresetModules[j];
                }
                PopulateButtonModules(slot);
                listlength = ModuleArchetypes.Count;
                Console.WriteLine("Select the button module in the {0} slot of your {1}:", slot.DescriptiveName, BaseModule.DescriptiveName);
                for (int i = 0; i < listlength; i++)
                {
                    Console.WriteLine("{0}) {1}", i + 1, ModuleArchetypes[i].DescriptiveName);
                    if (preset != null && ModuleArchetypes[i].ButtonFileName == preset)
                    {
                        selection = i + 1;
                    }
                }
                if (custom || selection == -1)
                {
                    selection = Program.PromptNumber(String.Format("{0} Module", slot.DescriptiveName), 1, listlength, selection);
                }
                else
                {
                    Console.WriteLine("Auto-selected {0}", ModuleArchetypes[selection - 1].DescriptiveName);
                }
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
        private void PopulatePresets()
        {
            string PresetsFilePath = string.Format("Customizers\\MTGR\\Presets.csv");
            if (File.Exists(PresetsFilePath))
            {
                StreamReader sr = File.OpenText(PresetsFilePath);
                string Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    PresetArchetypes.Add(PresetArchetype.FromCsv(Line));
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
