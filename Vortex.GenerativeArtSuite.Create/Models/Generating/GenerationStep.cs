using Vortex.GenerativeArtSuite.Common.Models;

namespace Vortex.GenerativeArtSuite.Create.Models.Generating
{
    public class GenerationStep
    {
        public GenerationStep(string layerName, string traitName)
        {
            Trait = new ERC721Trait
            {
                LayerName = layerName,
                TraitName = traitName,
            };
        }

        public ERC721Trait Trait { get; }
    }
}
