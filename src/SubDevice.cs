using System.Runtime.CompilerServices;

namespace VKB_Mobiflight_Definer
{
    internal class SubDevice
    {
        protected readonly List<Button> Buttons = [];
        protected readonly List<Led> Leds = [];
        public bool useLeds = true;
        public bool useButtons = true;
        public string DescriptiveName;
        public SubDevice(string DescName, string ButtonFile, string LedFile)
        {
            DescriptiveName = DescName;
            if(ButtonFile.Length > 0)
            {
                string ButtonFilePath = String.Format("Buttons\\{0}.csv", ButtonFile);
                if (File.Exists(ButtonFilePath))
                {
                    using StreamReader sr = File.OpenText(ButtonFilePath);
                    string? Line;
                    while ((Line = sr.ReadLine()) != null)
                    {
                        Buttons.Add(Button.FromCsv(Line));
                    }
                }
            }
            if (LedFile.Length > 0)
            {
                string LedFilePath = String.Format("LEDs\\{0}.csv", LedFile);
                if (File.Exists(LedFilePath))
                {
                    using StreamReader sr = File.OpenText(LedFilePath);
                    string? Line;
                    while ((Line = sr.ReadLine()) != null)
                    {
                        Leds.Add(Led.FromCsv(Line));
                    }
                }
            }
        }

        public List<Button> GetButtons()
        {
            if (useButtons)
                return Buttons;
            else
                return [];
        }
        public List<Led> GetLeds()
        {
            if (useLeds)
                return Leds;
            else
                return [];
        }
        public int GetNumberOfButtons()
        {
            return Buttons.Count;
        }
        public int GetNumberOfLeds()
        {
            return Leds.Count;
        }
        public string GetFirstButtonLabel()
        {
            return Buttons[0].GetButtonLabel();
        }
    }
}
