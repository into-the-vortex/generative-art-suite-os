using Newtonsoft.Json;

namespace Vortex.GenerativeArtSuite.Common.Models
{
    public struct ERC721Trait
    {
        /// <summary>
        /// Trait name
        /// </summary>
        [JsonProperty(PropertyName = "trait_type")]
        public string LayerName { get; set; }

        /// <summary>
        /// Trait value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string TraitName { get; set; }
    }
}
