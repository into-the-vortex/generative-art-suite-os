using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Create.Extensions;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;
using Vortex.GenerativeArtSuite.Create.Models.Settings;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.Models.Sessions
{
    public class Session
    {
        public Session(string name, SessionSettings settings)
        {
            Name = name;
            Settings = settings;
            Layers = new List<Layer>();
        }

        public string Name { get; }

        public SessionSettings Settings { get; }

        public List<Layer> Layers { get; }

        public bool CanMoveLayer(int from, int to)
        {
            if (from == to)
            {
                return false;
            }

            var mockList = new List<Layer>(Layers);
            mockList.RemoveAt(from);
            mockList.Insert(to, Layers[from]);

            foreach (var layer in mockList)
            {
                var seen = mockList.Take(mockList.IndexOf(layer)).Select(l => l.Name);
                if (!layer.GetDependencies().Select(d => d.Name).All(d => seen.Contains(d)))
                {
                    return false;
                }
            }

            return true;
        }

        public void OnLayerChanged(Dependency oldDep, Dependency newDep)
        {
            foreach (var layer in Layers)
            {
                if (layer.EditDependency(oldDep, newDep))
                {
                    layer.EnsureTraitsRemainValid(this);
                }
            }
        }

        public void OnLayerRemoved(int index)
        {
            var item = new Dependency(Layers[index]);
            foreach (var layer in Layers)
            {
                if (layer.RemoveDependency(item))
                {
                    layer.EnsureTraitsRemainValid(this);
                }
            }
        }

        public void OnTraitsChanged()
        {
            foreach (var layer in Layers)
            {
                layer.EnsureTraitsRemainValid(this);
            }
        }

        public Generation CreateRandomGeneration(int nextId)
        {
            var dna = string.Empty;
            var buildOrder = new List<GenerationStep>();

            foreach (var layer in Layers)
            {
                var trait = layer.SelectRandomTrait();

                if (layer.IncludeInDNA)
                {
                    dna += $"{layer.Name} = {trait.Name} ";
                }

                buildOrder.Add(trait.CreateGenerationStep(layer, buildOrder));
            }

            return new Generation(nextId, Hash(dna.Trim()), buildOrder);
        }

        public Generation CreateGeneration(int nextId, List<GenerationStep> steps)
        {
            var dna = string.Empty;
            var buildOrder = new List<GenerationStep>();

            foreach (var step in steps)
            {
                var layer = Layers.First(l => l.Name == step.Trait.LayerName);
                var trait = layer.Traits.First(t => t.Name == step.Trait.TraitName);

                if (layer.IncludeInDNA)
                {
                    dna += $"{layer.Name} = {trait.Name} ";
                }

                buildOrder.Add(trait.CreateGenerationStep(layer, buildOrder));
            }

            return new Generation(nextId, Hash(dna.Trim()), buildOrder);
        }

        public string HealthCheck()
        {
            var traits = Layers.SelectMany(l => l.Traits);

            var problems = new Dictionary<Trait, List<string>>();
            void RaiseProblems(Trait trait, List<string> traitProblems)
            {
                if (problems is null)
                {
                    throw new InvalidOperationException();
                }

                if (!traitProblems.Any())
                {
                    return;
                }

                if (!problems.ContainsKey(trait))
                {
                    problems[trait] = new List<string>();
                }

                problems[trait].AddRange(traitProblems);
            }

            foreach (var trait in traits)
            {
                RaiseProblems(trait, trait.GetProblems());
            }

            if(problems.Any())
            {
                string result = $"{Strings.HealthCheckFailed}:{Environment.NewLine}";

                foreach(var kvp in problems)
                {
                    result += $"{Environment.NewLine}{kvp.Key.Name}:{Environment.NewLine}";
                    foreach(var problem in kvp.Value)
                    {
                        result += $" - {problem}{Environment.NewLine}";
                    }
                }

                return result;
            }

            return string.Empty;
        }

        public List<Reference<string>> GetIconURIs()
        {
            return Layers.SelectMany(
                l => l.Traits.Select(
                    t => new Reference<string>(() => t.IconURI, v => t.IconURI = v)))
                .Where(m => File.Exists(m.Value))
                .ToList();
        }

        public List<Reference<string>> GetTraitURIs()
        {
            var result = new List<Reference<string>>();

            result.AddRange(
                Layers.SelectMany(
                    l => l.Traits.OfType<DrawnTrait>().Select(
                        t => new Reference<string>(() => t.TraitURI, v => t.TraitURI = v)))
                .Where(m => File.Exists(m.Value)));

            result.AddRange(
                Layers.SelectMany(
                    l => l.Traits.OfType<DependencyTrait>().SelectMany(
                        v => v.Variants.Select(
                            t => new Reference<string>(() => t.TraitURI, x => t.TraitURI = x))))
                .Where(m => File.Exists(m.Value)));

            return result;
        }

        public List<Reference<string>> GetMaskURIs()
        {
            var result = new List<Reference<string>>();

            result.AddRange(
                Layers.SelectMany(
                    l => l.Traits.OfType<DrawnTrait>().Select(
                        t => new Reference<string>(() => t.MaskURI, v => t.MaskURI = v)))
                .Where(m => File.Exists(m.Value)));

            result.AddRange(
                Layers.SelectMany(
                    l => l.Traits.OfType<DependencyTrait>().SelectMany(
                        v => v.Variants.Select(
                            t => new Reference<string>(() => t.MaskURI, x => t.MaskURI = x))))
                .Where(m => File.Exists(m.Value)));

            return result;
        }

        private static string Hash(string dna)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                return string.Concat(
                    algorithm.ComputeHash(Encoding.UTF8.GetBytes(dna))
                    .Select(item => item.ToString("x2", CultureInfo.InvariantCulture)));
            }
        }
    }
}
