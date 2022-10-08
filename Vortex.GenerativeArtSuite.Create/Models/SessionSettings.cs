namespace Vortex.GenerativeArtSuite.Create.Models
{
    public struct SessionSettings
    {
        public string OutputFolder { get; set; }

        public string NamePrefix { get; set; }

        public string DescriptionTemplate { get; set; }

        public string BaseURI { get; set; }

        public int CollectionSize { get; set; }
    }
}
