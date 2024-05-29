namespace VKB_Mobiflight_Definer
{
    internal class BaseArchetype(string DescName, string ButtonFile, string LedFile) : SubDeviceArchetype(DescName, ButtonFile, LedFile)
    {
        public Base Instantiate() => new(DescriptiveName, ButtonFileName, LedFileName);
        public static BaseArchetype FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            return new BaseArchetype(csvparts[0], csvparts[1], csvparts[2]);
        }

    }
}