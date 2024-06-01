using HidSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class JoystickDevice
    {
        public readonly int Pid;
        public readonly string InstanceName;
        public Base BaseType { get; private set; } = null;
        public readonly List<Module> Modules = new List<Module>();

        public JoystickDevice(HidDevice device)
        {
            Pid = device.ProductID;
            InstanceName = device.GetProductName();
        }

        public void SetBase(Base baseref)
        {
            BaseType = baseref;
        }
        public Base SetBase(BaseArchetype arch)
        {
            BaseType = arch.Instantiate();
            return BaseType;
        }
        public void AddModule(Module module)
        {
            Modules.Add(module);
        }
        public Module AddModule(ModuleArchetype arch)
        {
            Module module = arch.CreateModule();
            Modules.Add(module);
            return module;
        }
        public List<Button> GetButtons()
        {
            if (BaseType == null) return new List<Button>();
            List<Button> ret = new List<Button>(BaseType.GetButtons());
            foreach (var module in Modules)
            {
                ret.AddRange(module.GetButtons());
            }
            return ret;
        }
        public List<Led> GetLeds()
        {
            if (BaseType == null) return new List<Led>();
            List<Led> ret = new List<Led>(BaseType.GetLeds());
            foreach (var module in Modules)
            {
                ret.AddRange(module.GetLeds());
            }
            return ret;
        }
        public void WriteOut(StreamWriter sw)
        {
            bool firstitem = true;
            sw.WriteLine("{");
            sw.WriteLine("  \"$schema\": \"./mfjoystick.schema.json\",");
            sw.WriteLine("  \"InstanceName\": \"{0}\",", InstanceName.Trim());
            sw.WriteLine("  \"VendorId\": \"0x{0:X4}\",", 0x231D);
            sw.WriteLine("  \"ProductId\": \"0x{0:X4}\",", Pid);
            sw.Write    ("  \"Inputs\": [");
            foreach (var button in GetButtons())
            {
                if (firstitem)
                {
                    sw.WriteLine();
                    firstitem = false;
                }
                else
                {
                    sw.WriteLine(",");
                }
                button.WriteOut(sw);
            }
            if (!firstitem)
            {
                sw.WriteLine();
                sw.Write("  ");
            }
            sw.WriteLine("],");
            firstitem = true;
            sw.Write("  \"Outputs\": [");
            foreach (var led in GetLeds())
            {
                if (firstitem)
                {
                    sw.WriteLine();
                    firstitem = false;
                }
                else
                {
                    sw.WriteLine(",");
                }
                led.WriteOut(sw);
            }
            if (!firstitem)
            {
                sw.WriteLine();
                sw.Write("  ");
            }
            sw.WriteLine("]");
            sw.WriteLine("}");
        }
    }
}
