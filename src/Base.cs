using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class Base(string DescName, string ButtonFile, string LedFile) : SubDevice(DescName, ButtonFile, LedFile)
    {
        public static Base FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            return new Base(csvparts[0], csvparts[1], csvparts[2]);
        }
    }
}
