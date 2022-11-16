using System;
using Vortex.GenerativeArtSuite.Common.Staging;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Staging
{
    public class DrawnLayerStagingArea : LayerStagingArea
    {
        public DrawnLayerStagingArea(DrawnLayer model, Action onDependencyChanged)
            : base(model, onDependencyChanged)
        {
            Optional = new StagingProperty<bool>(optional => model.Optional = optional, () => model.Optional, onApply: model.EnsureOptional);
            AffectedByLayerMask = new StagingProperty<bool>(affectedByLayerMask => model.AffectedByLayerMask = affectedByLayerMask, () => model.AffectedByLayerMask);
        }

        public StagingProperty<bool> Optional { get; }

        public StagingProperty<bool> AffectedByLayerMask { get; }

        public override Layer Apply()
        {
            Optional.Apply();
            AffectedByLayerMask.Apply();

            return base.Apply();
        }

        public override bool IsDirty()
        {
            return Optional.IsDirty ||
                AffectedByLayerMask.IsDirty ||
                base.IsDirty();
        }
    }
}
