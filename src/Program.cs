
using HidSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Services;

namespace VKB_Mobiflight_Definer
{
    internal class Program
    {
        private static readonly List<BaseArchetype> BaseArchetypes = new List<BaseArchetype>();
        private static readonly List<ModuleArchetype> ModuleArchetypes = new List<ModuleArchetype>();
        private static void Main(string[] args)
        {
            IEnumerable<HidDevice> RawDevList = DeviceList.Local.GetHidDevices(vendorID: 0x231D);
            List<HidDevice> DevList = new List<HidDevice>();
            foreach(HidDevice dev in RawDevList)
            {
                try
                {
                    if (dev.GetReportDescriptor().FeatureReports.Count() == 0) continue;
                    DevList.Add(dev);
                }
                catch (NotSupportedException)
                {
                    // Probably a virtual controller, we can skip it
                }
            }
            Console.WriteLine("Please select your device from the following list:");
            int listlength = DevList.Count();
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

                Console.WriteLine(string.Format("{0})  \"{1}\" PID: {2:X4}, S/N: {3}", i + 1, ProductName, ProductId, SerialNumber));
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
            JoystickDevice Device = new JoystickDevice(ChosenDevice);
            PopulateBases();
            listlength = BaseArchetypes.Count;
            Console.WriteLine("Select your base:");
            for (int i = 0; i < listlength; i++)
            {
                Console.WriteLine("{0}) {1}", i + 1, BaseArchetypes[i].DescriptiveName);
            }
            selection = PromptNumber("Base", 1, listlength);
            Device.SetBase(BaseArchetypes[selection-1]);
            int buttonindex = 1;
            foreach (Button but in Device.BaseType.GetButtons()) 
            {
                if (but.GetButtonNumber() >= buttonindex) buttonindex = but.GetButtonNumber() + 1;
            }
            int ledindex = 10;
            foreach (Led led in Device.BaseType.GetLeds())
            {
                if (led.GetLedNumber() >= ledindex) ledindex = led.GetLedNumber() + 1;
            }
            PopulateModules();

            listlength = ModuleArchetypes.Count;
            int definableEncoders = Device.BaseType.GetNumberOfEncoders();
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
                    Module mod = Device.AddModule(ModuleArchetypes[modulechoice-1]);
                    int modbuttons = mod.GetNumberOfButtons();
                    if (modbuttons != 0)
                    {
                        if (buttonindex + modbuttons > 128) buttonindex = -1;
                        Console.WriteLine("Please enter button number of first button in module ({0}),", mod.GetFirstButtonLabel());
                        Console.WriteLine("or 0 if you do not want button labels for this module.");
                        Console.WriteLine("Use a button tester or logical button IDs in VKBDevCfg.");
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
                    definableEncoders += mod.GetNumberOfEncoders();
                    mod.Update();
                }
            }
            while (modulechoice != 0);
            if(definableEncoders != 0)
            {
                Console.WriteLine("Your device contains encoders.");
                Console.WriteLine("If your firmware is 2.17.9 or newer and you have Virtual BUS over USB enabled,");
                Console.WriteLine("you can use the Encoder API for improved responsiveness.");
                Console.WriteLine("Please enter 1 to use the Encoder API and 0 to skip it");
                if(PromptNumber("Use Encoder API", 0, 1, 1) == 1)
                {
                    int encId = 0;
                    int prevEncId = encId;
                    foreach (Encoder enc in Device.BaseType.GetEncoders())
                    {
                        Console.WriteLine($"Please enter the encoder ID of the {enc.EncoderLabel} encoder on your base device");
                        Console.WriteLine("Use the EncoderVisualizer tool to determine the encoder ID");
                        Console.WriteLine("Enter 0 to skip this encoder");
                        encId = PromptNumber("Encoder ID", 0, 0, prevEncId + 1);
                        if (encId > 0)
                        {
                            enc.encoderId = encId-1;
                            prevEncId = encId;
                            Device.BaseType.AddButtons(enc.GetButtons());
                        }
                    }
                    foreach(Module mod in Device.Modules)
                    foreach (Encoder enc in mod.GetEncoders())
                    {
                        Console.WriteLine($"Please enter the encoder ID of the {mod.LabelPrefix} {enc.EncoderLabel} encoder on your {mod.DescriptiveName}");
                        Console.WriteLine("Use the EncoderVisualizer tool to determine the encoder ID");
                        Console.WriteLine("Enter 0 to skip this encoder");
                        encId = PromptNumber("Encoder ID", 0, 0, prevEncId + 1);
                        if (encId > 0)
                        {
                            enc.encoderId = encId-1;
                            prevEncId = encId;
                            mod.AddButtons(enc.GetButtons());
                        }
                    }
                }
            }
            Console.WriteLine("Thank you for your input.");
            if(!System.IO.Directory.Exists("output"))
                System.IO.Directory.CreateDirectory("output");
            string filename = String.Format("output\\{0}.joystick.json",NameToCamelCase(Device.InstanceName.Trim()));
            StreamWriter sw = File.CreateText(filename);
            Device.WriteOut(sw);
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
                StreamReader sr = File.OpenText(BasesFilePath);
                string Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    BaseArchetypes.Add(BaseArchetype.FromCsv(Line));
                }
                sr.Close();
            }
        }
        private static void PopulateModules()
        {
            string ModulesFilePath = "Modules.csv";
            if (File.Exists(ModulesFilePath))
            {
                StreamReader sr = File.OpenText(ModulesFilePath);
                string Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    ModuleArchetypes.Add(ModuleArchetype.FromCsv(Line));
                }
                sr.Close();
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
                String entry = Console.ReadLine();
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
                if (c<256 && Char.IsLetterOrDigit(c))
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