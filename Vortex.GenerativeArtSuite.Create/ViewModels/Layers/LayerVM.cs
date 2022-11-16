using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models.Layers;
using Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public abstract class LayerVM : IViewModel<Layer>
    {
        protected LayerVM(Layer model, Action<Layer> editCallback, Action<Layer> deleteCallback)
        {
            Model = model;

            Settings = new List<string>
            {
                Model.IncludeInDNA ? Strings.IsDNAOn : Strings.IsDNAOff,
            };

            Dependencies = Model
                .Dependencies
                .Select(d => new DependencyVM(d, s => { throw new NotImplementedException(); }))
                .ToList();
            Edit = new DelegateCommand(() => editCallback(model));
            Delete = new DelegateCommand(() => deleteCallback(model));
        }

        public string Name => Model.Name;

        public List<string> Settings { get; }

        public List<DependencyVM> Dependencies { get; }

        public ICommand Edit { get; }

        public ICommand Delete { get; }

        public Layer Model { get; }
    }
}
