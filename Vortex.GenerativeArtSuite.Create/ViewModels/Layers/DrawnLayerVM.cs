using System;
using Vortex.GenerativeArtSuite.Create.Models.Layers;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public class DrawnLayerVM : LayerVM
    {
        public DrawnLayerVM(DrawnLayer model, Action<Layer> editCallback, Action<Layer> deleteCallback)
            : base(model, editCallback, deleteCallback)
        {
            Settings.AddRange(new string[]
            {
                model.Optional ? Strings.IsOptionalOn : Strings.IsOptionalOff,
                model.AffectedByLayerMask ? Strings.IsAffectedByMaskOn : Strings.IsAffectedByMaskOff,
            });
        }
    }
}
