using System;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public class PathingLayerVM : LayerVM
    {
        public PathingLayerVM(PathingLayer model, Action<Layer> editCallback, Action<Layer> deleteCallback)
            : base(model, editCallback, deleteCallback)
        {
        }
    }
}
