using System;
using System.Threading;
using Vortex.GenerativeArtSuite.Common.Models;

namespace Vortex.GenerativeArtSuite.Create.Models.Generating
{
    public class GenerationProcess : IGenerationProcess
    {
        private readonly CancellationTokenSource processTokenSource = new();
        private readonly ManualResetEvent pauseHandle = new(false);
        private readonly DateTime start = DateTime.Now;
        private readonly double maxProgress;

        private double progress;
        private bool isPaused;
        private DateTime lastPausePoint;
        private TimeSpan pauseOffset;

        public GenerationProcess(DebugConsole console, double maxProgress)
        {
            Console = console;
            this.maxProgress = maxProgress;
        }

        public event Action? ProcessComplete;

        public event Action? ErrorFound;

        public event Action<double>? ProgressMade;

        public DebugConsole Console { get; }

        public DateTime Start => start + pauseOffset;

        public bool IsCancellationRequested => processTokenSource.IsCancellationRequested;

        public CancellationToken Token => processTokenSource.Token;

        public void Complete()
        {
            CancelInternal();
            Console.Log("Generation successful");
            ProcessComplete?.Invoke();
        }

        public void Error(string error)
        {
            CancelInternal();
            Console.Error(error);
            ErrorFound?.Invoke();
        }

        public void Cancel()
        {
            CancelInternal();
            Console.Warn("Generation cancelled");
        }

        public void RespectCheckpoint()
        {
            if (isPaused)
            {
                pauseHandle.WaitOne();
            }

            Token.ThrowIfCancellationRequested();
        }

        public void ProgressBy(double weight)
        {
            progress += weight;
            ProgressMade?.Invoke(progress / maxProgress * 100);
        }

        public void SetPaused(bool paused)
        {
            isPaused = paused;

            if (paused)
            {
                pauseHandle.Reset();
                lastPausePoint = DateTime.Now;
            }
            else
            {
                pauseHandle.Set();
                pauseOffset += DateTime.Now - lastPausePoint;
            }
        }

        private void CancelInternal()
        {
            pauseHandle.Set();
            processTokenSource.Cancel();
        }
    }
}
