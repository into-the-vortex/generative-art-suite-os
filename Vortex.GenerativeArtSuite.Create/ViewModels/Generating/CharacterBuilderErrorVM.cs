namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generating
{
    public class CharacterBuilderErrorVM
    {
        public CharacterBuilderErrorVM(string error)
        {
            Error = error;
        }

        public string Error { get; }
    }
}
