namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generating
{
    public class CharacterBuilderResultVM
    {
        public CharacterBuilderResultVM(string json, byte[] image)
        {
            Json = json;
            Image = image;
        }

        public string Json { get; }

        public byte[] Image { get; }
    }
}
