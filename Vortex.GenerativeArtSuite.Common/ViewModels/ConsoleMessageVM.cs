namespace Vortex.GenerativeArtSuite.Common.ViewModels
{
    public abstract class ConsoleMessageVM
    {
        public ConsoleMessageVM(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
