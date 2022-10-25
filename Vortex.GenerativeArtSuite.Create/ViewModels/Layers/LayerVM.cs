using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public class LayerVM : IViewModel<Layer>
    {
        public LayerVM(Layer model, Action<Layer> editCallback, Action<Layer> deleteCallback)
        {
            Model = model;

            Settings = new List<string>
            {
                Model.Optional ? Strings.IsOptionalOn : Strings.IsOptionalOff,
                Model.IncludeInDNA ? Strings.IsDNAOn : Strings.IsDNAOff,
                Model.AffectedByLayerMask ? Strings.IsAffectedByMaskOn : Strings.IsAffectedByMaskOff,
            };
            Paths = new List<string>(model.Paths.Select(p => p.Path));
            Edit = new DelegateCommand(() => editCallback(model));
            Delete = new DelegateCommand(() => deleteCallback(model));
        }

        public string Name => Model.Name;

        public IEnumerable<string> Settings { get; }

        public IEnumerable<string> Paths { get; }

        public ICommand Edit { get; }

        public ICommand Delete { get; }

        public Layer Model { get; }
    }
}
