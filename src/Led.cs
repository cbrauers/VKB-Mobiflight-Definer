﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class Led
    {
        private readonly int ledNoInSubDevice;
        private readonly string ledBaseLabel;
        private readonly string ledBaseId;
        public int ledBaseNo = 1;
        public string ledLabelPrefix = "";
        public string ledIdPrefix = "";
        private readonly string colorChannels;

        public Led(int number, string colors, string label, string id)
        {
            ledNoInSubDevice = number;
            ledBaseLabel = label;
            ledBaseId = id;
            colorChannels = colors;
        }

        public static Led FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            int buttonNo = ((int)Convert.ToDecimal(csvparts[0]));
            string colors = csvparts[1];
            string label = csvparts[2];
            string id = csvparts[3];
            return new Led(buttonNo, colors, label, id);
        }

        public int GetLedNumber() { return ledBaseNo - 1 + ledNoInSubDevice; }
        public string GetLedLabel()
        {
            if (ledLabelPrefix.Length < 1)
            {
                return ledBaseLabel;
            }
            else
            {
                return ledLabelPrefix + " " + ledBaseLabel;
            }
        }
        public string GetLedLabel(char channel)
        {
            if (colorChannels.Length == 1) return GetLedLabel();
            else
            {
                return GetLedLabel() + " - " + ColorName(channel);
            }
        }
        public string GetLedId()
        {
            if (ledIdPrefix.Length < 1)
            {
                return ledBaseId;
            }
            else
            {
                return ledIdPrefix + "." + ledBaseId;
            }
        }
        public string GetLedId(char channel)
        {
            if (colorChannels.Length == 1) return GetLedId();
            else
            {
                return GetLedId() + "." + ColorName(channel);
            }
        }
        private static string ColorName(char channel)
        {
            switch (channel)
            {
                case 'R':
                    return "Red";
                case 'G':
                    return "Green";
                case 'B':
                    return "Blue";
                default:
                    return string.Format("{0}", channel);
            }
        }

        public void WriteOut(StreamWriter sw)
        {
            int channelno = 0;
            foreach (char c in colorChannels)
            {
                if (channelno > 0) sw.WriteLine(",");
                sw.WriteLine("    {");
                sw.WriteLine("      \"Label\": \"{0}\",", GetLedLabel(c));
                sw.WriteLine("      \"Id\": \"{0}\",", GetLedId(c));
                sw.WriteLine("      \"Byte\": {0},", GetLedNumber());
                sw.WriteLine("      \"Bit\": {0}", channelno);
                sw.Write("    }");
                channelno++;
            }
        }
    }
}
