using System.Collections.Generic;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Models.Traits
{
    public class PathingTrait : Trait
    {
        public PathingTrait()
            : base(string.Empty, string.Empty, DEFAULTWEIGHT)
        {
        }

        public PathingTrait(string name, string iconURI = "", int weight = DEFAULTWEIGHT)
            : base(name, iconURI, weight)
        {
        }

        public override GenerationStep CreateGenerationStep(Layer layer, List<GenerationStep> previousSteps)
        {
            return new GenerationStep(layer.Name, Name);
        }
    }
}
