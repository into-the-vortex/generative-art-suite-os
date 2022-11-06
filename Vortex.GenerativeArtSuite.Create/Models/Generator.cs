using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Console = Vortex.GenerativeArtSuite.Common.Models.Console;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public static class Generator
    {
        public static IGenerationProcess GenerateFor(Session session, Console console)
        {
            var enforcer = new UniqueDNAEnforcer();
            var process = new GenerationProcess();

            var toGenerate = (double)session.Settings.CollectionSize;

            console.Log("one");
            console.Warn("two");
            console.Log("three");
            console.Warn("four");
            console.Error("five");
            console.Error("six");
            console.Error("seven");

            Task.Run(() =>
            {
                while (enforcer.Count < toGenerate)
                {
                    var generation = session.CreateRandomGeneration();
                    var (wasUnique, failureThresholdMet) = enforcer.TryRegisterUnique(generation.DNA);

                    if (wasUnique)
                    {
                        generation.GenerateImage(enforcer.Count, session.Settings);
                        generation.GenerateJson(enforcer.Count, session.Settings);

                        process.InvokeProgressMade(enforcer.Count / toGenerate * 100);
                    }
                    else if (failureThresholdMet)
                    {
                        console.Error(Strings.GenerationFailureNotEnoughTraits);
                        process.InvokeErrorFound();
                        process.Cancel();
                    }

                    if (process.IsCancellationRequested)
                    {
                        break;
                    }
                }

                process.InvokeProcessComplete();
            });

            return process;
        }

        public static Bitmap CreateImage(List<GenerationStep> steps)
        {
            var images = steps.Select(s => Image.FromFile(s.ImageURI));
            var width = images.Max(i => i.Width);
            var height = images.Max(i => i.Height);

            var canvas = new Bitmap(width, height);
            using(var g = Graphics.FromImage(canvas))
            {
                foreach(var image in images)
                {
                    g.DrawImage(image, new Rectangle(0, 0, width, height));
                }
            }

            return canvas;
        }

        private class GenerationProcess : IGenerationProcess
        {
            private readonly CancellationTokenSource cts = new();

            public event Action? ProcessComplete;

            public event Action? ErrorFound;

            public event Action<double>? ProgressMade;

            public bool IsCancellationRequested => cts.IsCancellationRequested;

            public DateTime Start { get; } = DateTime.Now;

            public void Cancel()
            {
                cts.Cancel();
            }

            public void InvokeProcessComplete()
            {
                ProcessComplete?.Invoke();
            }

            public void InvokeProgressMade(double progress)
            {
                ProgressMade?.Invoke(progress);
            }

            public void InvokeErrorFound()
            {
                ErrorFound?.Invoke();
            }
        }

        private class UniqueDNAEnforcer
        {
            private const int MAXFAILURES = 100;

            private readonly ConcurrentBag<string> usedDNA = new();

            private int failureCounter;

            public int Count => usedDNA.Count;

            public (bool wasUnique, bool failureThresholdMet) TryRegisterUnique(string dna)
            {
                bool wasUnique = true;

                if (usedDNA.Contains(dna))
                {
                    Interlocked.Increment(ref failureCounter);
                    wasUnique = false;
                }
                else
                {
                    usedDNA.Add(dna);
                }

                return (wasUnique, failureCounter >= MAXFAILURES);
            }
        }
    }
}
