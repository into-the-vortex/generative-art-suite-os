using Vortex.GenerativeArtSuite.Common.Staging;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.Staging
{
    public class DrawnTraitStagingArea : TraitStagingArea
    {
        public DrawnTraitStagingArea(DrawnTrait model)
            : base(model)
        {
            TraitURI = new StagingProperty<string>(traitURI => model.TraitURI = traitURI, () => model.TraitURI);
            MaskURI = new StagingProperty<string>(maskURI => model.MaskURI = maskURI, () => model.MaskURI);
        }

        public StagingProperty<string> TraitURI { get; }

        public StagingProperty<string> MaskURI { get; }

        public override Trait Apply()
        {
            TraitURI.Apply();
            MaskURI.Apply();

            return base.Apply();
        }

        public override bool IsDirty()
        {
            return TraitURI.IsDirty &&
                MaskURI.IsDirty &&
                base.IsDirty();
        }
    }
}
