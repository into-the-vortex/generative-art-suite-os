namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class SessionSettings
    {
        public SessionSettings()
        {
            OutputFolder = string.Empty;
            NamePrefix = string.Empty;
            DescriptionTemplate = string.Empty;
            BaseURI = string.Empty;
            CollectionSize = 10000;
        }

        public string OutputFolder { get; set; }

        public string NamePrefix { get; set; }

        public string DescriptionTemplate { get; set; }

        public string BaseURI { get; set; }

        public int CollectionSize { get; set; }
    }
}
