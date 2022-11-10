using Vortex.GenerativeArtSuite.Common.Staging;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.Staging
{
    public class LayerStagingArea
    {
        private readonly Layer model;

        public LayerStagingArea(Layer model)
        {
            this.model = model;

            Name = new StagingProperty<string>(name => model.Name = name, () => model.Name);
            Optional = new StagingProperty<bool>(optional => model.Optional = optional, () => model.Optional, onApply: model.OnOptionalChanged);
            IncludeInDNA = new StagingProperty<bool>(includeInDNA => model.IncludeInDNA = includeInDNA, () => model.IncludeInDNA);
            AffectedByLayerMask = new StagingProperty<bool>(affectedByLayerMask => model.AffectedByLayerMask = affectedByLayerMask, () => model.AffectedByLayerMask);
            Paths = new StagingList<PathSelector>(model.Paths, onApply: model.OnTraitsInvalidated);
        }

        public StagingProperty<string> Name { get; }

        public StagingProperty<bool> Optional { get; }

        public StagingProperty<bool> IncludeInDNA { get; }

        public StagingProperty<bool> AffectedByLayerMask { get; }

        public StagingList<PathSelector> Paths { get; }

        public static bool CanApply()
        {
            return true;
        }

        public Layer Apply()
        {
            Name.Apply();
            Optional.Apply();
            IncludeInDNA.Apply();
            AffectedByLayerMask.Apply();
            Paths.Apply();

            return model;
        }

        public bool IsDirty()
        {
            return Name.IsDirty ||
                Optional.IsDirty ||
                IncludeInDNA.IsDirty ||
                AffectedByLayerMask.IsDirty ||
                Paths.IsDirty;
        }
    }
}
