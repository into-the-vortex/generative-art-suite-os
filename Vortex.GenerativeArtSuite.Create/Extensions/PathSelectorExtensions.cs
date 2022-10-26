using System.Collections.Generic;
using System.Linq;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.Extensions
{
    public static class PathSelectorExtensions
    {
        public static List<string> Variants(this List<PathSelector> selectors)
        {
            if (!selectors.Any())
            {
                return new();
            }

            var variants = selectors.First().Options;
            foreach (var owner in selectors.Skip(1))
            {
                variants = variants.SelectMany(pp => owner.Options.Select(word => string.Join(" - ", pp, word))).ToList();
            }

            return variants;
        }
    }
}
