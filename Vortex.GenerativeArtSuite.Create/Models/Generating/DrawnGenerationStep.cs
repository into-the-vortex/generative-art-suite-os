namespace Vortex.GenerativeArtSuite.Create.Models.Generating
{
    public class DrawnGenerationStep : GenerationStep
    {
        public DrawnGenerationStep(string layerName, string traitName, string imageURI)
            : base(layerName, traitName)
        {
            ImageURI = imageURI;
        }

        public string ImageURI { get; }
    }
}
