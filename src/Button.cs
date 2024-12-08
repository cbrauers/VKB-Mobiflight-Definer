using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class Button
    {
        private readonly int buttonNoInSubDevice;
        private readonly string buttonBaseLabel;
        public int buttonBaseId = 1;
        public string buttonLabelPrefix = "";

        public Button(int number, string label)
        {
            buttonNoInSubDevice = number;
            buttonBaseLabel = label;
        }

        public static Button FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            int buttonNo = ((int)Convert.ToDecimal(csvparts[0]));
            string label = csvparts[1];
            return new Button(buttonNo, label);
        }

        public Button Bake()
        {
            return new Button(GetButtonNumber(), GetButtonLabel());
        }

        public int GetButtonNumber() { 
            if(buttonBaseId < 1000)
                return buttonBaseId - 1 + buttonNoInSubDevice;
            else
                return buttonBaseId;
        }
        public string GetButtonLabel()
        {
            if (buttonLabelPrefix.Length < 1)
            {
                return buttonBaseLabel;
            }
            else
            {
                return buttonLabelPrefix + " " + buttonBaseLabel;
            }
        }
        public void WriteOut(StreamWriter sw)
        {
            sw.WriteLine("    {");
            sw.WriteLine("      \"Id\": {0},",GetButtonNumber());
            sw.WriteLine("      \"Type\": \"Button\",");
            sw.WriteLine("      \"Label\": \"{0}\"",GetButtonLabel());
            sw.Write    ("    }");
        }
    }
}
