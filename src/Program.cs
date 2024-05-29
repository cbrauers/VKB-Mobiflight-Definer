
using HidSharp;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

namespace VKB_Mobiflight_Definer
{
    internal class Program
    {
        private static readonly List<BaseArchetype> BaseArchetypes = [];
        private static readonly List<ModuleArchetype> ModuleArchetypes = [];
        private static void Main(string[] args)
        {
            HidDevice[] DevList = DeviceList.Local.GetHidDevices(vendorID: 0x231D).ToArray();
            Console.WriteLine("Please select your device from the following list:");
            int listlength = DevList.Length;
            string SerialNumber;
            for (int i = 0; i < listlength; i++)
            {
                HidDevice device = DevList[i];
                string ProductName = device.GetProductName();
                int ProductId = device.ProductID;
                SerialNumber = "None";
                try
                {
                    SerialNumber = device.GetSerialNumber();
                }
                catch (IOException) { }

                Console.WriteLine(string.Format("{0})  \"{1}\" PID: {2:X}, S/N: {3}", i + 1, ProductName, ProductId, SerialNumber));
            }
            int selection = PromptNumber("Device", 1, listlength);
            int deviceId = selection - 1;
            HidDevice ChosenDevice = DevList[deviceId];
            Console.WriteLine(string.Format("Selected Device {0}: {1}", selection, ChosenDevice.GetFriendlyName()));
            SerialNumber = "None";
            try
            {
                SerialNumber = ChosenDevice.GetSerialNumber();
            }
            catch (IOException) { }
            Console.WriteLine(string.Format("Product ID: {0:X}; Serial Number: {1}", ChosenDevice.ProductID, SerialNumber));
            JoystickDevice TestDevice = new(ChosenDevice);
            PopulateBases();
            listlength = BaseArchetypes.Count;
            Console.WriteLine("Select your base:");
            for (int i = 0; i < listlength; i++)
            {
                Console.WriteLine("{0}) {1}", i + 1, BaseArchetypes[i].DescriptiveName);
            }
            selection = PromptNumber("Base", 1, listlength);
            TestDevice.SetBase(BaseArchetypes[selection-1]);
            PopulateModules();

            listlength = ModuleArchetypes.Count;
            int buttonindex = -1;
            int ledindex = 10;
            int modulechoice;
            do
            {
                Console.WriteLine("Choose a module to add:");
                Console.WriteLine("0) No additional modules");
                for (int i = 0; i < listlength; i++)
                {
                    Console.WriteLine("{0}) {1}", i + 1, ModuleArchetypes[i].DescriptiveName);
                }
                modulechoice = PromptNumber("Add Module", 0, listlength);
                if(modulechoice != 0)
                {
                    Module mod = TestDevice.AddModule(ModuleArchetypes[modulechoice-1]);
                    int modbuttons = mod.GetNumberOfButtons();
                    if (modbuttons != 0)
                    {
                        if (buttonindex + modbuttons > 128) buttonindex = -1;
                        Console.WriteLine("Please enter button number of first button in module ({0}),", mod.GetFirstButtonLabel());
                        Console.WriteLine("or 0 if you do not want button labels for this module.");
                        int startbutton = PromptNumber("Button number", 0, 128 - modbuttons + 1, buttonindex);
                        if (startbutton != 0)
                        {
                            buttonindex = startbutton + modbuttons;
                            mod.ButtonBase = startbutton;
                            mod.useButtons = true;
                        }
                        else
                        {
                            buttonindex = -1;
                            mod.useButtons = false;
                        }
                    }
                    int modleds = mod.GetNumberOfLeds();
                    if (modleds != 0)
                    {
                        Console.WriteLine("Please enter the LED base displayed for your module in VKBDevCfg,");
                        Console.WriteLine("or 0 if you do not want an LED configuration for this module.");
                        int startled = PromptNumber("LED number", 0, 63, ledindex);
                        if (startled != 0)
                        {
                            mod.LedBase = startled;
                            mod.useLeds = true;
                        }
                        else
                        {
                            mod.useLeds = false;
                        }
                        ledindex = startled + modleds;
                        if (ledindex + modleds > 63) ledindex = -1;
                    }
                    mod.Update();
                }
            }
            while (modulechoice != 0);



            /*

            Module SCG = Module.FromCsv("SCG (LEDs only),SCG,SCG,,SCG");
            SCG.LedBase = 10;
            SCG.Update();
            TestDevice.AddModule(SCG);
            Module SEM = TestDevice.AddModule(ModuleArchetypes[1]);
            SEM.ButtonBase = 33;
            SEM.LedBase = 12;
            SEM.Update();
            SEM = TestDevice.AddModule(ModuleArchetypes[1]);
            SEM.ButtonBase = 57;
            SEM.LedBase = 20;
            SEM.Update();
            Module WW2Throttle = Module.FromCsv("THQ with WW2 grip,THQ,THQ,THQ_WW2,");
            WW2Throttle.ButtonBase = 81;
            WW2Throttle.Update();
            TestDevice.AddModule(WW2Throttle);
            Module FSMGA = Module.FromCsv("FSM.GA,FSM.GA,FSMGA,FSMGA,FSMGA");
            FSMGA.LedBase = 28;
            FSMGA.ButtonBase = 97;
            FSMGA.Update();
            TestDevice.AddModule(FSMGA);
            */

            Console.WriteLine("Thank you for your input.");
            if(!System.IO.Directory.Exists("output"))
                System.IO.Directory.CreateDirectory("output");
            string filename = String.Format("output\\{0}.joystick.json",NameToCamelCase(TestDevice.InstanceName.Trim()));
            StreamWriter sw = File.CreateText(filename);
            TestDevice.WriteOut(sw);
            sw.Flush();
            sw.Close();
            Console.WriteLine("Created definition file {0}", filename);
            Console.WriteLine("Please press enter to exit");
            Console.ReadLine();
        }
        private static void PopulateBases()
        {
            string BasesFilePath = "Bases.csv";
            if (File.Exists(BasesFilePath))
            {
                using StreamReader sr = File.OpenText(BasesFilePath);
                string? Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    BaseArchetypes.Add(BaseArchetype.FromCsv(Line));
                }
            }
        }
        private static void PopulateModules()
        {
            string ModulesFilePath = "Modules.csv";
            if (File.Exists(ModulesFilePath))
            {
                using StreamReader sr = File.OpenText(ModulesFilePath);
                string? Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    ModuleArchetypes.Add(ModuleArchetype.FromCsv(Line));
                }
            }
        }
        private static int PromptNumber(string message, int minvalue = 1, int maxvalue = 0, int defaultvalue = -1)
        {
            int selection;
            bool valid;
            do
            {
                if (maxvalue > minvalue)
                {
                    if (defaultvalue >= 0)
                    {
                        Console.Write("{0} ({1}-{2}, default: {3}): ", message, minvalue, maxvalue, defaultvalue);
                    }
                    else
                    {
                        Console.Write("{0} ({1}-{2}): ", message, minvalue, maxvalue);

                    }
                }
                else
                {
                    if (defaultvalue >= 0)
                    {
                        Console.Write("{0} (default: {1}): ", message, defaultvalue);
                    }
                    else
                    {
                        Console.Write("{0}: ", message);
                    }
                }
                String ? entry = Console.ReadLine();
                if (entry != null && entry == "" && defaultvalue >= 0)
                {
                    selection = defaultvalue;
                    valid = true;
                }
                else
                {
                    valid = int.TryParse(entry, out selection);
                }
                if (maxvalue > 0 && selection > maxvalue) valid = false;
                if (selection < minvalue) valid = false;
            } while (valid == false);
            return selection;
        }
        private static string NameToCamelCase(string name)
        {
            string outstring = "";
            bool rowofunderscores = true;
            foreach (char c in name)
            {
                if (Char.IsAsciiLetterOrDigit(c))
                {
                    outstring += (Char.ToLower(c));
                    rowofunderscores = false;
                }
                else if (rowofunderscores == false)
                {
                    outstring += "_";
                    rowofunderscores = true;
                }
                else
                {
                    rowofunderscores = true;
                }
            }
            return outstring;
        }
    }
}