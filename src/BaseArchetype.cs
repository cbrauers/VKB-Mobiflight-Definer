namespace VKB_Mobiflight_Definer
{
    internal class BaseArchetype : SubDeviceArchetype
    {
        public BaseArchetype(string DescName, string ButtonFile, string LedFile) : base(DescName, ButtonFile, LedFile)
        {
        }

        public Base Instantiate() => new Base(DescriptiveName, ButtonFileName, LedFileName);
        public static BaseArchetype FromCsv(string csv)
        {
            string[] csvparts = csv.Split(',');
            return new BaseArchetype(csvparts[0], csvparts[1], csvparts[2]);
        }

    }
}