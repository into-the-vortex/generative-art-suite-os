using Vortex.GenerativeArtSuite.Common.Staging;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.Staging
{
    public class DependencyTraitStagingArea : TraitStagingArea
    {
        public DependencyTraitStagingArea(DependencyTrait model)
            : base(model)
        {
            Variants = new StagingList<TraitVariant>(model.Variants);
        }

        public StagingList<TraitVariant> Variants { get; }

        public override Trait Apply()
        {
            Variants.Apply();

            return base.Apply();
        }

        public override bool IsDirty()
        {
            return Variants.IsDirty &&
                base.IsDirty();
        }
    }
}
