using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class Module : SubDevice
    {
        public string LabelPrefix;
        public string IdPrefix;
        public int LedBase = 10;
        public int ButtonBase = 1;

        public Module(string DescName, string Label, string Id, string ButtonFile, string LedFile, string PathPrefix = "") : base(DescName, ButtonFile, LedFile, PathPrefix)
        {
            LabelPrefix = Label;
            IdPrefix = Id;
        }

        public static Module FromCsv(string csv, string PathPrefix = "")
        {
            string[] csvparts = csv.Split(',');
            return new Module(csvparts[0], csvparts[1], csvparts[2], csvparts[3], csvparts[4], PathPrefix);
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
            foreach (var encoder in Encoders)
                encoder.buttonLabelPrefix = LabelPrefix;
        }
        public void BakeInto(Module module)
        {
            ButtonBase = module.GetButtons().Last().GetButtonNumber() + 1;
            Update();
            foreach (Button button in Buttons)
            {
                module.AddButton(button.Bake());
            }
        }
    }
}
