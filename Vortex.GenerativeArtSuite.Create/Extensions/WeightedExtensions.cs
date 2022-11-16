using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.Extensions
{
    public static class WeightedExtensions
    {
        // TODO: provide a seed in the settings?
        private static readonly Random Rnd = new();

        public static T SelectRandom<T>(this IEnumerable<T> weightedItems) where T : IWeighted
        {
            var sum = weightedItems.Sum(i => i.Weight);
            var selectedNumber = Rnd.Next(1, sum + 1);

            // Find the first item where the sum of all of the weights up too and including that item are greater than or equal too the random weight.
            return weightedItems.First(i => weightedItems.Take(weightedItems.IndexOf(i) + 1).Sum(i => i.Weight) >= selectedNumber);
        }
    }
}
