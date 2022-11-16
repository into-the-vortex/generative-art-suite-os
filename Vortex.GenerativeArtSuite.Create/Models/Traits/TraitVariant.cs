namespace Vortex.GenerativeArtSuite.Create.Models.Traits
{
    public class TraitVariant
    {
        public TraitVariant(string variantPath, string imagePath, string maskPath)
        {
            VariantPath = variantPath;
            TraitURI = imagePath;
            MaskURI = maskPath;
        }

        public string VariantPath { get; }

        public string TraitURI { get; set; }

        public string MaskURI { get; set; }
    }
}
