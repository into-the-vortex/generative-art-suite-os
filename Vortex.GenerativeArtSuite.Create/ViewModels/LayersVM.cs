using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.ViewModels.LayerDialogVM;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class LayersVM : SessionAwareVM
    {
        private readonly IDialogService dialogService;
        private IDisposable? layerListner;

        public LayersVM(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            Layers = new();
            AddLayer = new DelegateCommand(OnAdd);
        }

        public ObservableCollection<LayerVM> Layers { get; }

        public ICommand AddLayer { get; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            layerListner?.Dispose();
            layerListner = Layers.ConnectModelCollection(Session().Layers, (l) => new LayerVM(l, OnEdit, OnDelete));
        }

        private void OnAdd()
        {
            var param = new DialogParameters
            {
                { nameof(CreateLayerDialogVM.ExistingLayerNames), Layers.Select(l => l.Name).ToList() },
            };

            dialogService.ShowDialog(DialogVM.CreateLayerDialog, param, AddCallback);
        }

        private void OnEdit(Layer model)
        {
            var param = new DialogParameters
            {
                { nameof(Layer), model },
                { nameof(EditLayerDialogVM.ExistingLayerNames), Layers.Where(l => l.Name != model.Name).Select(l => l.Name).ToList() },
            };

            dialogService.ShowDialog(DialogVM.EditLayerDialog, param, EditCallback);
        }

        private void OnDelete(Layer model)
        {
            var param = new DialogParameters
            {
                { nameof(DeleteLayerDialogVM.Message), string.Format(CultureInfo.CurrentCulture, Strings.DeleteLayerConfirmation, model.Name) },
                { nameof(DeleteLayerDialogVM.Index), Session().Layers.IndexOf(model) },
                { nameof(Layer), model },
            };

            dialogService.ShowDialog(DialogVM.DeleteLayerDialog, param, DeleteCallback);
        }

        private void AddCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK && dialogResult.Parameters.TryGetValue(nameof(Layer), out Layer layer) &&
                dialogResult.Parameters.TryGetValue(nameof(CreateLayerDialogVM.Index), out int index))
            {
                Layers.Insert(index, new LayerVM(layer, OnEdit, OnDelete));
            }
        }

        private void EditCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                 dialogResult.Parameters.TryGetValue(nameof(EditLayerDialogVM.NewLayer), out Layer newLayer) &&
                  dialogResult.Parameters.TryGetValue(nameof(EditLayerDialogVM.OldLayer), out Layer oldLayer))
            {
                Layers.Replace(Layers.First(l => l.Model == oldLayer), new LayerVM(newLayer, OnEdit, OnDelete));
            }
        }

        private void DeleteCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK && dialogResult.Parameters.TryGetValue(nameof(DeleteLayerDialogVM.Index), out int index))
            {
                Layers.RemoveAt(index);
            }
        }
    }
}
