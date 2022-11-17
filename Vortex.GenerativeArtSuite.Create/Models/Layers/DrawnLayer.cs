using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Create.Extensions;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.Models.Layers
{
    public class DrawnLayer : Layer
    {
        [JsonProperty]
        private List<string> variants = new();

        public DrawnLayer()
            : this(string.Empty, true, new(), false, true)
        {
        }

        public DrawnLayer(string name, bool includeInDNA, List<Dependency> dependencies, bool optional, bool affectedByLayerMask)
            : base(name, includeInDNA, dependencies)
        {
            Optional = optional;
            AffectedByLayerMask = affectedByLayerMask;
        }

        public bool Optional { get; set; }

        public bool AffectedByLayerMask { get; set; }

        public override Trait CreateTrait(string name, string iconURI)
        {
            return variants.Any() ?
                new DependencyTrait(name, iconURI, variants) :
                new DrawnTrait(name, iconURI);
        }

        public override void EnsureTraitsRemainValid(Session session)
        {
            EnsureOptional();

            var dependencies = this.GetDependencies().Select(d => d.Name);
            var relatedLayers = session.Layers.Where(l => dependencies.Contains(l.Name));

            if (relatedLayers.Any())
            {
                ConvertToDependencyTraits();
                CorrectDependencyTraits(relatedLayers);
            }
            else
            {
                RemoveDependencyTraits();
            }
        }

        public void EnsureOptional()
        {
            if (Optional && Traits.FirstOrDefault() is not NoneTrait)
            {
                Traits.Insert(0, new NoneTrait());
            }
            else if (!Optional && Traits.FirstOrDefault() is NoneTrait)
            {
                Traits.RemoveAt(0);
            }
        }

        private void CorrectDependencyTraits(IEnumerable<Layer> relatedLayers)
        {
            variants = relatedLayers.Variants();
            foreach (var dpt in Traits.OfType<DependencyTrait>())
            {
                dpt.EnsureVariantsAreCorrect(variants);
            }
        }

        private void ConvertToDependencyTraits()
        {
            for (int i = 0; i < Traits.Count; i++)
            {
                if (Traits[i] is DrawnTrait dt)
                {
                    Traits[i] = new DependencyTrait(dt.Name, dt.IconURI, new(), dt.Weight);
                }

                if (Traits[i] is PathingTrait)
                {
                    throw new NotImplementedException(); // This should not currently occur.
                }
            }
        }

        private void RemoveDependencyTraits()
        {
            for (int i = 0; i < Traits.Count; i++)
            {
                if (Traits[i] is DependencyTrait dpt)
                {
                    Traits[i] = new DrawnTrait(dpt.Name, dpt.IconURI, dpt.Weight);
                }
            }
        }
    }
}
