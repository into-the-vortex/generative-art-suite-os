namespace Vortex.GenerativeArtSuite.Create.Models.Generating
{
    public interface IGenerationProcess
    {
        void RespectCheckpoint();

        void Error(string error);

        void Cancel();
    }
}
