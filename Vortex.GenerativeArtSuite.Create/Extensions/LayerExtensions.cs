using System.Collections.Generic;
using System.Linq;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Extensions
{
    public static class LayerExtensions
    {
        public static List<Dependency> GetDependencies(this IDependencyOwner layer)
        {
            var result = new List<Dependency>() { };
            CollectLayers(result, layer);
            return result;
        }

        public static List<string> Variants(this IEnumerable<Layer> layers)
        {
            if (!layers.Any())
            {
                return new();
            }

            var variants = layers.First().Traits.Select(t => t.Name).ToList();
            foreach (var owner in layers.Skip(1).Where(l => l.Traits.Any()))
            {
                variants = variants.SelectMany(pp => owner.Traits.Select(trait => string.Join(" - ", pp, trait.Name))).Distinct().ToList();
            }

            return variants;
        }

        private static void CollectLayers(List<Dependency> bucket, IDependencyOwner src)
        {
            foreach (var dep in src.Dependencies)
            {
                bucket.AddUnique(dep);
                CollectLayers(bucket, dep);
            }
        }
    }
}
