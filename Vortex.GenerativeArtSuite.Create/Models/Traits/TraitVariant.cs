using System;
using Newtonsoft.Json;

namespace Vortex.GenerativeArtSuite.Create.Models.Traits
{
    public class TraitVariant
    {
        [JsonProperty(propertyName: "TraitURI")]
        private string traitURI;

        [JsonProperty(propertyName: "MaskURI")]
        private string maskURI;

        public TraitVariant(string variantPath, string traitURI, string maskURI)
        {
            VariantPath = variantPath;
            this.traitURI = traitURI;
            this.maskURI = maskURI;
        }

        public string VariantPath { get; }

        [JsonIgnore]
        public string TraitURI
        {
            get => Environment.ExpandEnvironmentVariables(traitURI);
            set => traitURI = value;
        }

        [JsonIgnore]
        public string MaskURI
        {
            get => Environment.ExpandEnvironmentVariables(maskURI);
            set => maskURI = value;
        }
    }
}
