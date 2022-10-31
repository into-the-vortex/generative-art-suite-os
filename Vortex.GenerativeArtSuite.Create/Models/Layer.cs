using System.Collections.Generic;
using System.Linq;
using Vortex.GenerativeArtSuite.Create.Extensions;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public sealed class Layer
    {
        public Layer()
            : this(string.Empty, false, true, true, new())
        {
        }

        public Layer(string name, bool optional, bool includeInDNA, bool affectedByLayerMask, List<PathSelector> paths)
        {
            Name = name;
            Optional = optional;
            IncludeInDNA = includeInDNA;
            AffectedByLayerMask = affectedByLayerMask;
            Paths = paths;
            Traits = new();
        }

        public string Name { get; set; }

        public bool Optional { get; set; }

        public bool IncludeInDNA { get; set; }

        public bool AffectedByLayerMask { get; set; } = true;

        public List<PathSelector> Paths { get; }

        public List<Trait> Traits { get; }

        public Trait SelectRandomTrait()
        {
            return Traits.SelectRandom();
        }

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
