using Vortex.GenerativeArtSuite.Common.Models;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public struct GenerationStep
    {
        public GenerationStep(string layerName, string traitName, string imageURI, string? maskURI)
        {
            Trait = new ERC721Trait
            {
                LayerName = layerName,
                TraitName = traitName,
            };
            ImageURI = imageURI;
            MaskURI = maskURI;
        }

        public ERC721Trait Trait { get; }

        public string ImageURI { get; }

        public string? MaskURI { get; }
    }
}
