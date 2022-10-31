namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class TraitVariant : IWeighted
    {
        public TraitVariant(string displayName, string? imagePath, string? maskPath, int weight)
        {
            DisplayName = displayName;
            ImagePath = imagePath;
            MaskPath = maskPath;
            Weight = weight;
        }

        public string DisplayName { get; }

        public string? ImagePath { get; set; }

        public string? MaskPath { get; set; }

        public int Weight { get; set; }
    }
}
