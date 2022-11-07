using System;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public interface IGenerationProcess
    {
        event Action? ProcessComplete;

        event Action? ErrorFound;

        event Action<double>? ProgressMade;

        DateTime Start { get; }

        void SetPaused(bool paused);

        void Cancel();
    }
}
