using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DynamicData;
using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Models.Layers;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;
using Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public class LayersVM : SessionAwareVM, IDropTarget
    {
        private readonly IDialogService dialogService;
        private readonly ISessionProvider sessionProvider;

        private IDisposable? layerListener;

        public LayersVM(IDialogService dialogService, ISessionProvider sessionProvider)
            : base(sessionProvider, dialogService)
        {
            this.dialogService = dialogService;
            this.sessionProvider = sessionProvider;

            Layers = new();
            AddLayer = new DelegateCommand(OnAdd);
        }

        public ObservableCollection<LayerVM> Layers { get; }

        public ICommand AddLayer { get; }

        public void DragOver(IDropInfo dropInfo)
        {
            HandleDropInfo(
                dropInfo,
                (oldIndex, newIndex) =>
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    dropInfo.Effects = DragDropEffects.Move;
                },
                () =>
                {
                    dropInfo.Effects = DragDropEffects.None;
                });
        }

        public void Drop(IDropInfo dropInfo)
        {
            HandleDropInfo(
                dropInfo,
                (oldIndex, newIndex) =>
                {
                    Layers.Move(oldIndex, newIndex);
                },
                () =>
                {
                });
        }

        protected override void ResetOnSessionChanged()
        {
            layerListener?.Dispose();
            layerListener = Layers.ConnectModelCollection(sessionProvider.Session().Layers, (l) => CreateVM(l));
        }

        private void HandleDropInfo(IDropInfo dropInfo, Action<int, int> onValid, Action? onInvalid = null)
        {
            if (dropInfo.Data is LayerVM sourceItem &&
                dropInfo.TargetItem is LayerVM targetItem)
            {
                var oldIndex = Layers.IndexOf(sourceItem);
                var newIndex = Layers.IndexOf(targetItem);

                if (sessionProvider.Session().CanMoveLayer(oldIndex, newIndex))
                {
                    onValid(oldIndex, newIndex);
                }
                else
                {
                    onInvalid?.Invoke();
                }
            }
        }

        private void OnAdd()
        {
            var param = new DialogParameters
            {
                { LayerDialogParameters.ExistingLayers, Layers.Select(l => l.Name).ToList() },
                { LayerDialogParameters.PreviousLayers, Layers.Select(l => l.Model).ToList() },
            };

            dialogService.ShowDialog(DialogVM.CreateLayerDialog, param, AddCallback);
        }

        private void OnEdit(Layer model)
        {
            var param = new DialogParameters
            {
                { LayerDialogParameters.ExistingLayers, Layers.Where(l => l.Name != model.Name).Select(l => l.Name).ToList() },
                { LayerDialogParameters.PreviousLayers, Layers.Take(sessionProvider.Session().Layers.IndexOf(model)).Select(l => l.Model).ToList() },
                { LayerDialogParameters.LayerStagingArea, CreateStaging(model) },
            };

            dialogService.ShowDialog(DialogVM.EditLayerDialog, param, EditCallback);
        }

        private void OnDelete(Layer model)
        {
            var param = new DialogParameters
            {
                { LayerDialogParameters.Message, string.Format(CultureInfo.CurrentCulture, Strings.DeleteLayerConfirmation, model.Name) },
                { LayerDialogParameters.Index, sessionProvider.Session().Layers.IndexOf(model) },
            };

            dialogService.ShowDialog(DialogVM.DeleteLayerDialog, param, DeleteCallback);
        }

        private void AddCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(LayerDialogParameters.CreatedLayer, out Layer model))
            {
                model.EnsureTraitsRemainValid(sessionProvider.Session());
                Layers.Add(CreateVM(model));
            }
        }

        private void EditCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(LayerDialogParameters.EditedLayer, out Layer layer))
            {
                Layers.Replace(Layers.First(l => l.Model == layer), CreateVM(layer));
            }
        }

        private void DeleteCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK && dialogResult.Parameters.TryGetValue(DeleteDialogVM.Index, out int index))
            {
                sessionProvider.Session().OnLayerRemoved(index);
                Layers.RemoveAt(index);

                ResetOnSessionChanged();
            }
        }

        private LayerVM CreateVM(Layer model)
        {
            switch (model)
            {
                case DrawnLayer dl:
                    return new DrawnLayerVM(dl, OnEdit, OnDelete);
                case PathingLayer pl:
                    return new PathingLayerVM(pl, OnEdit, OnDelete);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private LayerStagingArea CreateStaging(Layer model)
        {
            var oldDependency = new Dependency(model);
            var onDependencyChanged = new Action(() =>
            {
                sessionProvider.Session().OnLayerChanged(oldDependency, new Dependency(model));
                ResetOnSessionChanged();
            });

            switch (model)
            {
                case DrawnLayer dl:
                    return new DrawnLayerStagingArea(dl, onDependencyChanged);
                case PathingLayer pl:
                    return new PathingLayerStagingArea(pl, onDependencyChanged);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
