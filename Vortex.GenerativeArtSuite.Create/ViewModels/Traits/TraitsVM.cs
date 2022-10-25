﻿using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using DynamicData;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;
using Vortex.GenerativeArtSuite.Create.ViewModels.Layers;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitsVM : IViewModel<Layer>
    {
        private readonly IDialogService dialogService;

        public TraitsVM(IDialogService dialogService, Layer layer)
        {
            this.dialogService = dialogService;

            Model = layer;
            Add = new DelegateCommand(OnAdd);
            TraitVms.ConnectModelCollection(layer.Traits, (t) => new TraitVM(t, OnEdit, OnDelete));
        }

        public ICommand Add { get; }

        public ObservableCollection<TraitVM> TraitVms { get; } = new();

        public Layer Model { get; }

        private void OnAdd()
        {
            var param = new DialogParameters
            {
                { nameof(TraitDialogVM.ExistingTraitNames), Model.Traits.Select(l => l.Name).ToList() },
                { nameof(TraitStagingArea), new TraitStagingArea(Model.CreateTrait()) },
            };

            dialogService.ShowDialog(DialogVM.CreateTraitDialog, param, AddCallback);
        }

        private void AddCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(nameof(Trait), out Trait trait))
            {
                TraitVms.Add(new TraitVM(trait, OnEdit, OnDelete));
            }
        }

        private void OnEdit(Trait model)
        {
            var param = new DialogParameters
            {
                { nameof(LayerDialogVM.ExistingLayerNames), Model.Traits.Where(l => l.Name != model.Name).Select(l => l.Name).ToList() },
                { nameof(TraitStagingArea), new TraitStagingArea(model) },
            };

            dialogService.ShowDialog(DialogVM.EditTraitDialog, param, EditCallback);
        }

        private void EditCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(nameof(Trait), out Trait trait))
            {
                TraitVms.Replace(TraitVms.First(l => l.Model == trait), new TraitVM(trait, OnEdit, OnDelete));
            }
        }

        private void OnDelete(Trait model)
        {
            var param = new DialogParameters
            {
                { nameof(DeleteDialogVM.Message), string.Format(CultureInfo.CurrentCulture, Strings.DeleteTraitConfirmation, model.Name) },
                { nameof(DeleteDialogVM.Index), Model.Traits.IndexOf(model) },
            };

            dialogService.ShowDialog(DialogVM.DeleteTraitDialog, param, DeleteCallback);
        }

        private void DeleteCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(nameof(DeleteDialogVM.Index), out int index))
            {
                TraitVms.RemoveAt(index);
            }
        }
    }
}
