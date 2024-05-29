using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKB_Mobiflight_Definer
{
    internal class SubDeviceArchetype(string DescName, string ButtonFile, string LedFile)
    {
        public string DescriptiveName = DescName;
        public string ButtonFileName = ButtonFile;
        public string LedFileName = LedFile;
    }
}
