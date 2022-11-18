using System.Collections.Generic;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Models.Traits
{
    public class NoneTrait : Trait
    {
        public const string NAME = "None";

        public NoneTrait()
            : base(NAME, @"pack://application:,,,/Resources/None.png", DEFAULTWEIGHT)
        {
        }

        public override GenerationStep CreateGenerationStep(Layer layer, List<GenerationStep> previousSteps)
        {
            return new GenerationStep(layer.Name, Name);
        }

        public override List<string> GetProblems()
        {
            return new();
        }
    }
}
