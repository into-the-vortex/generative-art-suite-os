using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Vortex.GenerativeArtSuite.Create.Extensions;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.Models.Settings
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

        public bool CanGenerate()
        {
            return GetTraitURIs().All(uri => File.Exists(uri));
        }

        public List<string> GetIconURIs()
        {
            return Layers.SelectMany(l => l.Traits.Select(t => t.IconURI)).ToList();
        }

        public List<string> GetTraitURIs()
        {
            var result = new List<string>();

            result.AddRange(Layers.SelectMany(l => l.Traits.OfType<DrawnTrait>().Select(t => t.TraitURI)));
            result.AddRange(Layers.SelectMany(l => l.Traits.OfType<DependencyTrait>().SelectMany(v => v.Variants.Select(v => v.TraitURI))));

            return result;
        }

        public List<string> GetMaskURIs()
        {
            var result = new List<string>();

            result.AddRange(Layers.SelectMany(l => l.Traits.OfType<DrawnTrait>().Select(t => t.MaskURI)));
            result.AddRange(Layers.SelectMany(l => l.Traits.OfType<DependencyTrait>().SelectMany(v => v.Variants.Select(v => v.MaskURI))));

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
