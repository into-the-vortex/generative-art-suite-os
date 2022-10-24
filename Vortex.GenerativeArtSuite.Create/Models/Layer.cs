using System.Collections.Generic;
using System.Linq;
using Vortex.GenerativeArtSuite.Create.Extensions;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public sealed class Layer
    {
        public Layer(string name, bool optional, bool includeInDNA, bool affectedByLayerMask, List<PathSelector> paths)
        {
            Name = name;
            Optional = optional;
            IncludeInDNA = includeInDNA;
            AffectedByLayerMask = affectedByLayerMask;
            Paths = paths;
        }

        public string Name { get; set; } = string.Empty;

        public bool Optional { get; set; }

        public bool IncludeInDNA { get; set; } = true;

        public bool AffectedByLayerMask { get; set; } = true;

        public List<PathSelector> Paths { get; } = new();

        public List<Trait> Traits { get; } = new();

        public Trait CreateTrait() => Trait.Default(Paths.Variants());

        public void OnOptionalChanged()
        {
            if (Optional && Traits.FirstOrDefault()?.Name != Trait.NONENAME)
            {
                Traits.Insert(0, Trait.None(Paths.Variants()));
            }
            else if (!Optional && Traits.FirstOrDefault()?.Name == Trait.NONENAME)
            {
                Traits.RemoveAt(0);
            }
        }

        public void OnTraitsInvalidated()
        {
            var variants = Paths.Variants();

            foreach (var trait in Traits)
            {
                trait.OnVariantsChanged(variants);
            }
        }
    }
}
