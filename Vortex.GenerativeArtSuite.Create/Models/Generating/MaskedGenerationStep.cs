namespace Vortex.GenerativeArtSuite.Create.Models.Generating
{
    public class MaskedGenerationStep : DrawnGenerationStep
    {
        public MaskedGenerationStep(string layerName, string traitName, string imageURI, string maskURI)
            : base(layerName, traitName, imageURI)
        {
            MaskURI = maskURI;
        }

        public string MaskURI { get; }
    }
}
