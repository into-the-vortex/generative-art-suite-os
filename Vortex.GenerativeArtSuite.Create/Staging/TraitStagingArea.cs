using Vortex.GenerativeArtSuite.Common.Staging;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.Staging
{
    public abstract class TraitStagingArea
    {
        private readonly Trait model;

        public TraitStagingArea(Trait model)
        {
            this.model = model;

            Name = new StagingProperty<string>(name => model.Name = name, () => model.Name);
            IconURI = new StagingProperty<string>(iconURI => model.IconURI = iconURI, () => model.IconURI);
            Weight = new StagingProperty<int>(includeInDNA => model.Weight = includeInDNA, () => model.Weight);
        }

        public StagingProperty<string> Name { get; }

        public StagingProperty<string> IconURI { get; }

        public StagingProperty<int> Weight { get; }

        public virtual Trait Apply()
        {
            Name.Apply();
            IconURI.Apply();
            Weight.Apply();

            return model;
        }

        public virtual bool IsDirty()
        {
            return Name.IsDirty ||
                IconURI.IsDirty ||
                Weight.IsDirty;
        }
    }
}
