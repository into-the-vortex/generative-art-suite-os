using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class LayerVM : IViewModel<Layer>
    {
        public LayerVM(Layer model, Action<Layer> editCallback, Action<Layer> deleteCallback)
        {
            Model = model;

            Fork = new DelegateCommand(OnAddSubLayer);
            Edit = new DelegateCommand(() => editCallback(model));
            Delete = new DelegateCommand(() => deleteCallback(model));
            BranchVMs = new ObservableCollection<SubLayerVM>(model.Branches.Select(b => new SubLayerVM(b)));
        }

        public string Name => $"{Strings.NameLabel} {Model.Name}";

        public string Info => $" - ({(Model.Optional ? Strings.IsOptionalOn : Strings.IsOptionalOff)} | {(Model.IncludeInDNA ? Strings.IsDNAOn : Strings.IsDNAOff)})";

        public ObservableCollection<SubLayerVM> BranchVMs { get; }

        public ICommand Fork { get; }

        public ICommand Edit { get; }

        public ICommand Delete { get; }

        public Layer Model { get; }

        private void OnAddSubLayer()
        {
        }
    }
}
