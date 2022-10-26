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
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
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
                { nameof(LayerDialogVM.ExistingLayerNames), Layers.Select(l => l.Name).ToList() },
                { nameof(LayerStagingArea), new LayerStagingArea(new()) },
            };

            dialogService.ShowDialog(DialogVM.CreateLayerDialog, param, AddCallback);
        }

        private void OnEdit(Layer model)
        {
            var param = new DialogParameters
            {
                { nameof(LayerDialogVM.ExistingLayerNames), Layers.Where(l => l.Name != model.Name).Select(l => l.Name).ToList() },
                { nameof(LayerStagingArea), new LayerStagingArea(model) },
            };

            dialogService.ShowDialog(DialogVM.EditLayerDialog, param, EditCallback);
        }

        private void OnDelete(Layer model)
        {
            var param = new DialogParameters
            {
                { nameof(DeleteLayerDialogVM.Message), string.Format(CultureInfo.CurrentCulture, Strings.DeleteLayerConfirmation, model.Name) },
                { nameof(DeleteDialogVM.Index), Session().Layers.IndexOf(model) },
            };

            dialogService.ShowDialog(DialogVM.DeleteLayerDialog, param, DeleteCallback);
        }

        private void AddCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(nameof(CreateLayerDialogVM.Index), out int index) &&
                dialogResult.Parameters.TryGetValue(nameof(Layer), out Layer layer))
            {
                Layers.Insert(index, new LayerVM(layer, OnEdit, OnDelete));
            }
        }

        private void EditCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(nameof(Layer), out Layer layer))
            {
                Layers.Replace(Layers.First(l => l.Model == layer), new LayerVM(layer, OnEdit, OnDelete));
            }
        }

        private void DeleteCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK && dialogResult.Parameters.TryGetValue(nameof(DeleteDialogVM.Index), out int index))
            {
                Layers.RemoveAt(index);
            }
        }
    }
}
