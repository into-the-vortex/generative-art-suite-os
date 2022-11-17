using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Models.Traits
{
    public class DrawnTrait : Trait
    {
        [JsonProperty(propertyName: "TraitURI")]
        private string traitURI;

        [JsonProperty(propertyName: "MaskURI")]
        private string maskURI;

        public DrawnTrait()
            : this(string.Empty, string.Empty, DEFAULTWEIGHT, string.Empty, string.Empty)
        {
        }

        public DrawnTrait(string name, string iconURI, int weight = DEFAULTWEIGHT, string traitURI = "", string maskURI = "")
            : base(name, iconURI, weight)
        {
            this.traitURI = traitURI;
            this.maskURI = maskURI;
        }

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

        public override GenerationStep CreateGenerationStep(Layer layer, List<GenerationStep> previousSteps)
        {
            return File.Exists(MaskURI) ?
                new MaskedGenerationStep(layer.Name, Name, TraitURI, MaskURI) :
                new DrawnGenerationStep(layer.Name, Name, TraitURI);
        }
    }
}
