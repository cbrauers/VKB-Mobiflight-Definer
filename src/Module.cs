using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class Module(string DescName, string Label, string Id, string ButtonFile, string LedFile) : SubDevice(DescName, ButtonFile, LedFile)
    {
        public string LabelPrefix = Label;
        public string IdPrefix = Id;
        public int LedBase = 10;
        public int ButtonBase = 1;
        public static Module FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            return new Module(csvparts[0], csvparts[1], csvparts[2], csvparts[3], csvparts[4]);
        }
        public void Update()
        {
            foreach (var button in Buttons)
            {
                button.buttonLabelPrefix = LabelPrefix;
                button.buttonBaseId = ButtonBase;
            }
            foreach (var led in Leds)
            {
                led.ledLabelPrefix = LabelPrefix;
                led.ledIdPrefix = IdPrefix;
                led.ledBaseNo = LedBase;
            }
        }
    }
}
