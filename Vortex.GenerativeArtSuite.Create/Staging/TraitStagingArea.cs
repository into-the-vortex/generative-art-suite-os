using Vortex.GenerativeArtSuite.Common.Staging;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.Staging
{
    public class TraitStagingArea
    {
        private readonly Trait model;

        public TraitStagingArea(Trait model)
        {
            Name = new StagingProperty<string>(name => model.Name = name, () => model.Name);
            IconURI = new StagingProperty<string?>(optional => model.IconURI = optional, () => model.IconURI);
            Weight = new StagingProperty<int>(includeInDNA => model.Weight = includeInDNA, () => model.Weight);
            Variants = new StagingList<TraitVariant>(model.Variants);
            this.model = model;
        }

        public StagingProperty<string> Name { get; }

        public StagingProperty<string?> IconURI { get; }

        public StagingProperty<int> Weight { get; }

        public StagingList<TraitVariant> Variants { get; }

        public static bool CanApply()
        {
            return true;
        }

        public Trait Apply()
        {
            Name.Apply();
            IconURI.Apply();
            Weight.Apply();
            Variants.Apply();

            return model;
        }

        public bool IsDirty()
        {
            return Name.IsDirty ||
                IconURI.IsDirty ||
                Weight.IsDirty ||
                Variants.IsDirty;
        }
    }
}
