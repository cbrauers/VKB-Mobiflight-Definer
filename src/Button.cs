using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class Button(int number, string label)
    {
        private readonly int buttonNoInSubDevice = number;
        private readonly string buttonBaseLabel = label;
        public int buttonBaseId = 0;
        public string buttonLabelPrefix = "";

        public static Button FromCsv(string csv)
        {
            string[] csvparts = csv.Split(",");
            int buttonNo = ((int)Convert.ToDecimal(csvparts[0]));
            string label = csvparts[1];
            return new Button(buttonNo, label);
        }

        public int GetButtonNumber() { return buttonBaseId - 1 + buttonNoInSubDevice; }
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
