using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vortex.GenerativeArtSuite.Common.Models;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public static class Generator
    {
        public static IGenerationProcess GenerateFor(Session session, DebugConsole console)
        {
            var process = new GenerationProcess(console);

            Task.Run(() =>
            {
                // TODO: either clean up the folder or restore the progress.
                process.DefineUniqueTokens(session, console);



                if (!process.IsCancellationRequested)
                {
                    process.InvokeProcessComplete();
                }
            });

            return process;
        }

        private static List<Generation> DefineUniqueTokens(this GenerationProcess gp, Session session, DebugConsole console)
        {
            var result = new List<Generation>();

            if (gp.IsCancellationRequested)
            {
                return result;
            }

            var usedDNA = new List<string>();
            var currentDuplicateDNA = 0;
            var maxDuplicateDNA = 100; // TODO: Get from setting.

            console.Log($"Creating {session.Settings.CollectionSize} unique DNA sequences");

            while (usedDNA.Count != session.Settings.CollectionSize)
            {
                gp.RespectPaused();

                var attempt = session.CreateRandomGeneration();

                if (usedDNA.Contains(attempt.DNA))
                {
                    currentDuplicateDNA++;

                    if (currentDuplicateDNA > maxDuplicateDNA)
                    {
                        console.Error("Could not create enough unique DNA sequences, add more variety or try again.");
                        gp.InvokeErrorFound();
                    }
                }
                else
                {
                    result.Add(attempt);
                    usedDNA.Add(attempt.DNA);
                }

                if (gp.IsCancellationRequested)
                {
                    break;
                }

                Thread.Sleep(1);
                gp.InvokeProgressMade(usedDNA.Count / 10000.0 * 100);
            }

            if (result.Count == session.Settings.CollectionSize)
            {
                console.Log($"Successfully created {session.Settings.CollectionSize} unique DNA sequences");
            }

            return result;
        }

        private class GenerationProcess : IGenerationProcess
        {
            private readonly CancellationTokenSource processTokenSource = new();
            private readonly ManualResetEvent pauseHandle = new(false);
            private readonly DateTime start = DateTime.Now;
            private readonly DebugConsole console;

            private bool isPaused;
            private DateTime lastPausePoint;
            private TimeSpan pauseOffset;

            public GenerationProcess(DebugConsole console)
            {
                this.console = console;
            }

            public event Action? ProcessComplete;

            public event Action? ErrorFound;

            public event Action<double>? ProgressMade;

            public DateTime Start => start + pauseOffset;

            public bool IsCancellationRequested => processTokenSource.IsCancellationRequested;

            public CancellationToken Token => processTokenSource.Token;

            public void Cancel()
            {
                console.Log("Generation cancelled");

                OnCancel();
            }

            public void RespectPaused()
            {
                if (isPaused)
                {
                    pauseHandle.WaitOne();
                }
            }

            public void InvokeProcessComplete()
            {
                OnCancel();

                ProcessComplete?.Invoke();
            }

            public void InvokeProgressMade(double progress)
            {
                ProgressMade?.Invoke(progress);
            }

            public void InvokeErrorFound()
            {
                OnCancel();

                ErrorFound?.Invoke();
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

            private void OnCancel()
            {
                pauseHandle.Set();
                processTokenSource.Cancel();
            }
        }
    }
}
