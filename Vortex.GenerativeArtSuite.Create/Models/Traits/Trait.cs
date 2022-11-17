using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Models.Traits
{
    public abstract class Trait : IWeighted
    {
        [JsonProperty(propertyName: "IconURI")]
        private string iconURI;

        public const int DEFAULTWEIGHT = 50;

        protected Trait()
            : this(string.Empty, string.Empty, DEFAULTWEIGHT)
        {
        }

        protected Trait(string name, string iconURI, int weight)
        {
            Name = name;
            this.iconURI = iconURI;
            Weight = weight;
        }

        public string Name { get; set; }

        [JsonIgnore]
        public string IconURI
        {
            get => Environment.ExpandEnvironmentVariables(iconURI);
            set => iconURI = value;
        }

        public int Weight { get; set; }

        public abstract GenerationStep CreateGenerationStep(Layer layer, List<GenerationStep> previousSteps);
    }
}
