using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class SubDeviceArchetype
    {
        public string DescriptiveName;
        public string ButtonFileName;
        public string LedFileName;

        public SubDeviceArchetype(string DescName, string ButtonFile, string LedFile)
        {
            DescriptiveName = DescName;
            ButtonFileName = ButtonFile;
            LedFileName = LedFile;
        }
    }
}
