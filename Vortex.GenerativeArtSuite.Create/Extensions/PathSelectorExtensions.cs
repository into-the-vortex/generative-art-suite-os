using System.Collections.Generic;
using System.Linq;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.Extensions
{
    public static class PathSelectorExtensions
    {
        public static List<string> Variants(this List<PathSelector> selectors)
        {
            static List<string> recurse(List<string> partialPath, PathSelector ps)
            {
                var result = new List<string>();

                foreach (var pp in partialPath)
                {
                    foreach (var word in ps.Options)
                    {
                        result.Add(string.Join(" - ", pp, word));
                    }
                }

                return result;
            }

            var variants = selectors.First().Options;
            foreach (var owner in selectors.Skip(1))
            {
                variants = recurse(variants, owner);
            }

            return variants;
        }
    }
}
