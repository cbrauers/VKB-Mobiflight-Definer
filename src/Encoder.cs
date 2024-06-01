using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VKB_Mobiflight_Definer
{
    internal class Encoder
    {
        public readonly string EncoderLabel;
        private readonly string buttonDecBaseLabel;
        private readonly string buttonIncBaseLabel;
        public int encoderId = 0;
        public string buttonLabelPrefix = "";
        public Encoder(string enclabel, string declabel, string inclabel)
        {
            EncoderLabel = enclabel;
            buttonDecBaseLabel = declabel;
            buttonIncBaseLabel = inclabel;
        }
        public static Encoder FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            return new Encoder(csvparts[0], csvparts[1], csvparts[2]);
        }
        public List<Button> GetButtons()
        {
            Button buttonDec = new Button(1000 + (encoderId * 10), buttonDecBaseLabel);
            Button buttonInc = new Button(1000 + (encoderId * 10) + 5, buttonIncBaseLabel);
            buttonDec.buttonLabelPrefix = buttonLabelPrefix;
            buttonInc.buttonLabelPrefix = buttonLabelPrefix;
            return new List<Button>
            {
                buttonDec,
                buttonInc
            };
        }
    }
}
