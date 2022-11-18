using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Models.Generating
{
    public static class Generator
    {
        private const double WEIGHTSUNIQUE = 0.005;
        private const double WEIGHTJSON = 0.05;
        private const double WEIGHTIMAGE = 0.945;

        public static GenerationProcess GenerateFor(Session session, DebugConsole console)
        {
            var process = new GenerationProcess(console, session.Settings.CollectionSize);

            Task.Run(() =>
            {
                try
                {
                    process.RespectCheckpoint();

                    process.EnsureHealthCheck(session);

                    process.RespectCheckpoint();

                    var toGenerate = process.DefineUniqueTokens(session);

                    Task.WaitAll(process.CreateFiles(toGenerate, session.Settings), process.Token);

                    if (!process.IsCancellationRequested)
                    {
                        process.Complete();
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    process.Error(e.Message);
                }
            });

            return process;
        }

        private static void EnsureHealthCheck(this GenerationProcess gp, Session session)
        {
            var healthResult = session.HealthCheck();
            if (!string.IsNullOrEmpty(healthResult))
            {
                throw new InvalidOperationException($"{Strings.HealthCheckFailed}{Strings.HealthCheckFailedDetails}");
            }
        }

        private static List<Generation> DefineUniqueTokens(this GenerationProcess gp, Session session)
        {
            var result = new List<Generation>();

            if (gp.IsCancellationRequested)
            {
                return result;
            }

            var usedDNA = new List<string>();
            var currentDuplicateDNA = 0;
            var maxDuplicateDNA = 100; // TODO: Get from setting.

            gp.Console.Log($"Creating {session.Settings.CollectionSize} unique DNA sequences");

            while (usedDNA.Count != session.Settings.CollectionSize)
            {
                gp.RespectCheckpoint();

                var attempt = session.CreateRandomGeneration(usedDNA.Count + 1);

                if (usedDNA.Contains(attempt.DNA))
                {
                    currentDuplicateDNA++;

                    if (currentDuplicateDNA > maxDuplicateDNA)
                    {
                        gp.Error("Could not create enough unique DNA sequences, add more variety or try again.");
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
                gp.Console.Log($"Successfully created {session.Settings.CollectionSize} unique DNA sequences");
            }

            return result;
        }

        private static Task[] CreateFiles(this GenerationProcess gp, List<Generation> toGenerate, SessionSettings settings)
        {
            return toGenerate.Select(tg => Task.Run(
                () =>
            {
                try
                {
                    gp.RespectCheckpoint();
                    tg.SaveGeneratedMetadata(gp, settings.JsonOutputFolder(), settings);
                    gp.ProgressBy(WEIGHTJSON);

                    gp.RespectCheckpoint();
                    tg.SaveGeneratedImage(gp, settings.ImageOutputFolder());
                    gp.ProgressBy(WEIGHTIMAGE);

                    gp.Console.Log($"Successfully created asset #{tg.Id}");
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    gp.Error(e.Message);
                }
            }, gp.Token)).ToArray();
        }
    }
}
