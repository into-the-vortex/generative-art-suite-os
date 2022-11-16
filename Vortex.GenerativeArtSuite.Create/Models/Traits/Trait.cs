using System.Collections.Generic;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Models.Traits
{
    public abstract class Trait : IWeighted
    {
        public const int DEFAULTWEIGHT = 50;

        protected Trait()
            : this(string.Empty, string.Empty, DEFAULTWEIGHT)
        {
        }

        protected Trait(string name, string iconURI, int weight)
        {
            Name = name;
            IconURI = iconURI;
            Weight = weight;
        }

        public string Name { get; set; }

        public string IconURI { get; set; }

        public int Weight { get; set; }

        public abstract GenerationStep CreateGenerationStep(Layer layer, List<GenerationStep> previousSteps);
    }
}
