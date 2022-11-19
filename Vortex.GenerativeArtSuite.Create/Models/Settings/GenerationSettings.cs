using Newtonsoft.Json;

namespace Vortex.GenerativeArtSuite.Create.Models.Settings
{
    public class GenerationSettings
    {
        public GenerationSettings()
        {
            NamePrefix = string.Empty;
            DescriptionTemplate = string.Empty;
            BaseURI = string.Empty;
            ExternalUrl = string.Empty;
            CollectionSize = 10000;
        }

        [JsonProperty]
        public string NamePrefix { get; set; }

        [JsonProperty]
        public string DescriptionTemplate { get; set; }

        [JsonProperty]
        public string BaseURI { get; set; }

        [JsonProperty]
        public string ExternalUrl { get; set; }

        [JsonProperty]
        public int CollectionSize { get; set; }
    }
}
