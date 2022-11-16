using System;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.Staging
{
    public class PathingLayerStagingArea : LayerStagingArea
    {
        public PathingLayerStagingArea(PathingLayer model, Action onDependencyChanged)
            : base(model, onDependencyChanged)
        {
        }
    }
}
