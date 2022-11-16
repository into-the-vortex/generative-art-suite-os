using System.Collections.Generic;
using Newtonsoft.Json;

namespace Vortex.GenerativeArtSuite.Common.Models
{
    // Source: https://docs.opensea.io/docs/metadata-standards
    public struct ERC721Metadata
    {
        /// <summary>
        /// This is the URL to the image of the item. Can be just about any type of image (including SVGs, which will be cached into PNGs by OpenSea), and can be IPFS URLs or paths. We recommend using a 350 x 350 image.
        /// </summary>
        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }

        /// <summary>
        /// A human readable description of the item. Markdown is supported.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Name of the item.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// This is the URL that will appear below the asset's image on OpenSea and will allow users to leave OpenSea and view the item on your site.
        /// </summary>
        [JsonProperty(PropertyName = "external_url")]
        public string? ExternalUrl { get; set; }

        /// <summary>
        /// These are the attributes for the item, which will show up on the OpenSea page for the item.
        /// </summary>
        [JsonProperty(PropertyName = "attributes")]
        public IEnumerable<ERC721Trait> Attributes { get; set; }

        /// <summary>
        /// Not shown on opensea, the unique token Id
        /// </summary>
        [JsonProperty(PropertyName = "edition")]
        public int Id { get; set; }

        /// <summary>
        /// Not shown on opensea, unique asset DNA.
        /// </summary>
        [JsonProperty(PropertyName = "dna")]
        public string Dna { get; set; }

        /// <summary>
        /// Not shown on opensea, the generation timestamp.
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        public long Date { get; set; }

        /// <summary>
        /// Not shown on opensea, the generation compiler.
        /// </summary>
        [JsonProperty(PropertyName = "compiler")]
        public string Compiler { get; set; }
    }
}
