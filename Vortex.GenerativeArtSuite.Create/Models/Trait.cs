using System.Collections.Generic;
using System.Linq;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class Trait
    {
        public const string NONENAME = "None";
        public const string DEFAULTVARIANTNAME = "Default";
        public const int DEFAULTWEIGHT = 50;

        public Trait(string name, string? iconURI, int weight, List<TraitVariant> variants)
        {
            Name = name;
            IconURI = iconURI;
            Weight = weight;
            Variants = variants;
        }

        public string Name { get; set; }

        public string? IconURI { get; set; }

        public int Weight { get; set; }

        public List<TraitVariant> Variants { get; set; }

        public static Trait None(List<string> variants) => new(NONENAME, null, DEFAULTWEIGHT, CreateDefaults(variants));

        public static Trait Default(List<string> variants) => new(string.Empty, null, DEFAULTWEIGHT, CreateDefaults(variants));

        public static List<TraitVariant> CreateDefaults(List<string> variants)
        {
            if (variants.Any())
            {
                return variants.Select(v => new TraitVariant(v, null, null, DEFAULTWEIGHT)).ToList();
            }
            else
            {
                return new List<TraitVariant>
                {
                    new TraitVariant(DEFAULTVARIANTNAME, null, null, DEFAULTWEIGHT),
                };
            }
        }

        public void OnVariantsChanged(List<string> variants)
        {
            Variants = CreateDefaults(variants);
        }
    }
}
