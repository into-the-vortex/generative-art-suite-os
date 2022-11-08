using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vortex.GenerativeArtSuite.Common.Models;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public static class Generator
    {
        private const double WEIGHTSUNIQUE = 0.000025;
        private const double WEIGHTJSON = 0.000365;
        private const double WEIGHTIMAGE = 0.999610;

        public static IGenerationProcess GenerateFor(Session session, DebugConsole console)
        {
            var process = new GenerationProcess(console, session.Settings.CollectionSize);

            Task.Run(() =>
            {
                try
                {
                    // TODO: either clean up the folder or restore the progress.
                    var toGenerate = process.DefineUniqueTokens(session, console);

                    Task.WaitAll(process.CreateFiles(toGenerate, session.Settings), process.Token);

                    if (!process.IsCancellationRequested)
                    {
                        process.InvokeProcessComplete();
                    }
                }
                catch (OperationCanceledException)
                {
                    console.Warn("Generation cancelled");
                }
                catch (Exception e)
                {
                    console.Error(e.Message);
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
                gp.RespectCheckpoint();

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

                gp.ProgressBy(WEIGHTSUNIQUE);
            }

            if (result.Count == session.Settings.CollectionSize)
            {
                console.Log($"Successfully created {session.Settings.CollectionSize} unique DNA sequences");
            }

            return result;
        }

        private static Task[] CreateFiles(this GenerationProcess gp, List<Generation> toGenerate, SessionSettings settings)
        {
            return toGenerate.Select((tg, index) => Task.Run(
                () =>
            {
                try
                {
                    var id = index + 1;

                    gp.RespectCheckpoint();
                    tg.GenerateJson(gp, id, settings);
                    gp.ProgressBy(WEIGHTJSON);

                    gp.RespectCheckpoint();
                    tg.GenerateImage(gp, id, settings);
                    gp.ProgressBy(WEIGHTIMAGE);

                    gp.Console.Log($"Successfully created asset #{id}");
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    gp.Console.Error(e.Message);
                }
            }, gp.Token)).ToArray();
        }

        private class GenerationProcess : IGenerationProcess
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

            public void Cancel()
            {
                pauseHandle.Set();
                processTokenSource.Cancel();
            }

            public void RespectCheckpoint()
            {
                if (isPaused)
                {
                    pauseHandle.WaitOne();
                }

                Token.ThrowIfCancellationRequested();
            }

            public void InvokeProcessComplete()
            {
                Cancel();
                Console.Log("Generation successful");
                ProcessComplete?.Invoke();
            }

            public void ProgressBy(double weight)
            {
                progress += weight;
                ProgressMade?.Invoke(progress / maxProgress * 100);
            }

            public void InvokeErrorFound()
            {
                Cancel();

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
        }
    }
}
