using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class LayersVM : SessionAwareVM
    {
        private readonly IDialogService dialogService;

        public LayersVM(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            Layers = new();
            AddLayer = new DelegateCommand(OnAddLayer);
        }

        public ObservableCollection<LayerVM> Layers { get; }

        public ICommand AddLayer { get; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Layers.Clear();
            Layers.AddRange(Session().Layers.Select(l => new LayerVM(l, dialogService, EditCallback, DeleteCallback)));
        }

        private void OnAddLayer()
        {
            var param = new DialogParameters
            {
                { nameof(CreateLayerDialogVM.ExistingLayerNames), Layers.Select(l => l.Name).ToList() },
            };

            dialogService.ShowDialog(DialogVM.CreateLayerDialog, param, AddCallback);
        }

        private void AddCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK && dialogResult.Parameters.TryGetValue(nameof(Layer), out Layer layer))
            {
                Layers.Insert(layer.Index, new LayerVM(layer, dialogService, EditCallback, DeleteCallback));
                Session().Layers.Insert(layer.Index, layer);
            }
        }

        private void EditCallback(IDialogResult dialogResult)
        {

        }

        private void DeleteCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK && dialogResult.Parameters.TryGetValue(nameof(Layer), out Layer layer))
            {
                Layers.RemoveAt(layer.Index);
                Session().Layers.RemoveAt(layer.Index);
            }
        }
    }
}
