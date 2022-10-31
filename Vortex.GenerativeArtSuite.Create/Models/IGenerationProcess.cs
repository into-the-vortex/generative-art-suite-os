using System;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public interface IGenerationProcess
    {
        event Action? ProcessComplete;

        event Action<double>? ProgressMade;

        event Action<string>? ErrorFound;

        void Cancel();
    }
}
