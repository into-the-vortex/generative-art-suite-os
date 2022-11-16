using System.Collections.Generic;
using System.IO;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Models.Traits
{
    public class DrawnTrait : Trait
    {
        public DrawnTrait()
            : this(string.Empty, string.Empty, DEFAULTWEIGHT, string.Empty, string.Empty)
        {
        }

        public DrawnTrait(string name, string iconURI, int weight = DEFAULTWEIGHT, string traitURI = "", string maskURI = "")
            : base(name, iconURI, weight)
        {
            TraitURI = traitURI;
            MaskURI = maskURI;
        }

        public string TraitURI { get; set; }

        public string MaskURI { get; set; }

        public override GenerationStep CreateGenerationStep(Layer layer, List<GenerationStep> previousSteps)
        {
            return File.Exists(MaskURI) ?
                new MaskedGenerationStep(layer.Name, Name, TraitURI, MaskURI) :
                new DrawnGenerationStep(layer.Name, Name, TraitURI);
        }
    }
}
