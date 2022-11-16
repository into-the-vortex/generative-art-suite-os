using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vortex.GenerativeArtSuite.Create.Extensions;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Models.Traits
{
    public class DependencyTrait : Trait
    {
        public DependencyTrait()
            : this(string.Empty, string.Empty, new(), DEFAULTWEIGHT, string.Empty, string.Empty)
        {
        }

        public DependencyTrait(string name, string iconURI, List<string> variants, int weight = DEFAULTWEIGHT, string traitURI = "", string maskURI = "")
            : base(name, iconURI, weight)
        {
            Variants = variants
                .Select(v => new TraitVariant(v, string.Empty, string.Empty))
                .ToList();
        }

        public List<TraitVariant> Variants { get; }

        public override GenerationStep CreateGenerationStep(Layer layer, List<GenerationStep> previousSteps)
        {
            var dependencies = layer.GetDependencies().Select(d => d.Name);
            var expected = string.Join(" - ", previousSteps.Where(ps => dependencies.Contains(ps.Trait.LayerName)).Select(ps => ps.Trait.TraitName));

            var variant = Variants.First(v => v.VariantPath == expected);

            return File.Exists(variant.MaskURI) ?
                new MaskedGenerationStep(layer.Name, Name, variant.TraitURI, variant.MaskURI) :
                new DrawnGenerationStep(layer.Name, Name, variant.TraitURI);
        }

        public void EnsureVariantsAreCorrect(List<string> expected)
        {
            Variants.RemoveAll(v => !expected.Contains(v.VariantPath));
            Variants.AddRange(expected.Where(d => !Variants.Any(v => v.VariantPath == d)).Select(d => new TraitVariant(d, string.Empty, string.Empty)));
        }
    }
}
