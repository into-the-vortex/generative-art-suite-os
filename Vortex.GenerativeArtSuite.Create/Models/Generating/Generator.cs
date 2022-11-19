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
            var process = new GenerationProcess(console, session.GenerationSettings.CollectionSize);

            Task.Run(() =>
            {
                try
                {
                    process.RespectCheckpoint();

                    process.EnsureHealthCheck(session);

                    process.RespectCheckpoint();

                    var toGenerate = process.DefineUniqueTokens(session);

                    Task.WaitAll(process.CreateFiles(toGenerate, session.UserSettings, session.GenerationSettings), process.Token);

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
                gp.Error($"{Strings.HealthCheckFailed}{Strings.HealthCheckFailedDetails}");
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

            gp.Console.Log($"Creating {session.GenerationSettings.CollectionSize} unique DNA sequences");

            while (usedDNA.Count != session.GenerationSettings.CollectionSize)
            {
                gp.RespectCheckpoint();

                var attempt = session.CreateRandomGeneration(usedDNA.Count + 1);

                if (usedDNA.Contains(attempt.DNA))
                {
                    currentDuplicateDNA++;

                    if (currentDuplicateDNA > session.UserSettings.MaxDuplicateDNAThreshold)
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

            if (result.Count == session.GenerationSettings.CollectionSize)
            {
                gp.Console.Log($"Successfully created {session.GenerationSettings.CollectionSize} unique DNA sequences");
            }

            return result;
        }

        private static Task[] CreateFiles(
            this GenerationProcess gp,
            List<Generation> toGenerate,
            UserSettings userSettings,
            GenerationSettings generationSettings)
        {
            return toGenerate.Select(tg => Task.Run(
                () =>
            {
                try
                {
                    gp.RespectCheckpoint();
                    tg.SaveGeneratedMetadata(gp, userSettings.JsonOutputFolder(), generationSettings);
                    gp.ProgressBy(WEIGHTJSON);

                    gp.RespectCheckpoint();
                    tg.SaveGeneratedImage(gp, userSettings.ImageOutputFolder());
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
