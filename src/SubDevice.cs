using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace VKB_Mobiflight_Definer
{
    internal class SubDevice
    {
        protected readonly List<Button> Buttons = new List<Button>();
        protected readonly List<Led> Leds = new List<Led>();
        protected readonly List<Encoder> Encoders = new List<Encoder>();
        public bool useLeds = true;
        public bool useButtons = true;
        public string DescriptiveName;
        public SubDevice(string DescName, string ButtonFile, string LedFile, string PathPrefix = "")
        {
            DescriptiveName = DescName;
            if(ButtonFile.Length > 0)
            {
                string ButtonFilePath = String.Format("{1}Buttons\\{0}.csv", ButtonFile, PathPrefix);
                if (File.Exists(ButtonFilePath))
                {
                    StreamReader sr = File.OpenText(ButtonFilePath);
                    string Line;
                    while ((Line = sr.ReadLine()) != null)
                    {
                        Buttons.Add(Button.FromCsv(Line));
                    }
                    sr.Close();
                }
                string EncoderFilePath = String.Format("{1}Encoders\\{0}.csv", ButtonFile, PathPrefix);
                if (File.Exists(EncoderFilePath))
                {
                    StreamReader sr = File.OpenText(EncoderFilePath);
                    string Line;
                    while ((Line = sr.ReadLine()) != null)
                    {
                       Encoders.Add(Encoder.FromCsv(Line));
                    }
                    sr.Close();
                }
            }
            if (LedFile.Length > 0)
            {
                string LedFilePath = String.Format("{1}LEDs\\{0}.csv", LedFile, PathPrefix);
                if (File.Exists(LedFilePath))
                {
                    StreamReader sr = File.OpenText(LedFilePath);
                    string Line;
                    while ((Line = sr.ReadLine()) != null)
                    {
                        Leds.Add(Led.FromCsv(Line));
                    }
                    sr.Close();
                }
            }
        }

        public List<Button> GetButtons()
        {
            if (useButtons)
                return Buttons;
            else
                return new List<Button>();
        }
        public List<Led> GetLeds()
        {
            if (useLeds)
                return Leds;
            else
                return new List<Led>();
        }
        public List<Encoder> GetEncoders()
        {
            return Encoders;
        }
        public void AddButton(Button button)
        {
            Buttons.Add(button);
        }
        public void AddButtons(IEnumerable<Button> buttons)
        {
            Buttons.AddRange(buttons);
        }
        public int GetNumberOfButtons()
        {
            return Buttons.Count;
        }
        public int GetNumberOfLeds()
        {
            return Leds.Count;
        }
        public int GetNumberOfEncoders()
        {
            return Encoders.Count;
        }
        public string GetFirstButtonLabel()
        {
            return Buttons[0].GetButtonLabel();
        }
    }
}
