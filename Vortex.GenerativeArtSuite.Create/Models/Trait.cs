using System.Collections.Generic;
using System.Linq;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class Trait
    {
        public const string NONENAME = "None";

        public Trait(string name, string iconURI, double weight, List<TraitVariant> variants)
        {
            Name = name;
            IconURI = iconURI;
            Weight = weight;
            Variants = variants;
        }

        public string Name { get; set; }

        public string IconURI { get; set; }

        public double Weight { get; set; }

        public List<TraitVariant> Variants { get; set; }

        public static Trait None(List<string> variants) => new(NONENAME, string.Empty, 1, CreateDefaults(variants));

        public static List<TraitVariant> CreateDefaults(List<string> variants)
        {
            if (variants.Any())
            {
                return variants.Select(v => new TraitVariant(v, string.Empty, string.Empty, 1)).ToList();
            }
            else
            {
                return new List<TraitVariant>
                {
                    new TraitVariant(Strings.DefaultVariant, string.Empty, string.Empty, 1),
                };
            }
        }

        public void OnVariantsChanged(List<string> variants)
        {
            Variants = CreateDefaults(variants);
        }
    }
}
