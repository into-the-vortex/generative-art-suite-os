using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Vortex.GenerativeArtSuite.Common.Extensions;

namespace Vortex.GenerativeArtSuite.Create.Models
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

        public Generation CreateRandomGeneration(int nextId)
        {
            var dna = string.Empty;
            var buildOrder = new List<GenerationStep>();
            var chosenPaths = new List<string>();

            foreach (var layer in Layers)
            {
                var trait = layer.SelectRandomTrait();
                var variant = trait.SelectRandomVariant(chosenPaths);

                if (layer.IncludeInDNA)
                {
                    dna += $"{layer.Name} = {trait.Name}/{variant.DisplayName} ";
                }

                if (trait.Name != Trait.NONENAME)
                {
                    buildOrder.Add(
                        new(layer.Name,
                            trait.Name,
                            variant.ImagePath ?? throw new InvalidOperationException(),
                            variant.MaskPath));
                }

                if (variant.DisplayName != Trait.DEFAULTVARIANTNAME)
                {
                    chosenPaths.AddUnique(variant.DisplayName);
                }
            }

            return new Generation(nextId, Hash(dna.Trim()), buildOrder, chosenPaths);
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
