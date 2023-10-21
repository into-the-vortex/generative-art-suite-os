using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DynamicData;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models.Layers;
using Vortex.GenerativeArtSuite.Create.Models.Traits;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class LayerTraitsVM : IViewModel<Layer>
    {
        private readonly ISessionProvider sessionProvider;
        private readonly IDialogService dialogService;

        public LayerTraitsVM(ISessionProvider sessionProvider, IDialogService dialogService, Layer layer)
        {
            this.sessionProvider = sessionProvider;
            this.dialogService = dialogService;

            Model = layer;
            Add = new DelegateCommand(OnAdd);
            Drop = new DelegateCommand<DragEventArgs>(OnDrop);
            TraitIconVMs.ConnectModelCollection(layer.Traits, (t) => new TraitIconVM(t, OnEdit, OnDelete));
        }

        public ICommand Add { get; }

        public ICommand Drop { get; }

        public ObservableCollection<TraitIconVM> TraitIconVMs { get; } = new();

        public Layer Model { get; }

        private static TraitStagingArea CreateStaging(Trait model)
        {
            switch (model)
            {
                case DrawnTrait dt:
                    return new DrawnTraitStagingArea(dt);
                case PathingTrait pt:
                    return new PathingTraitStagingArea(pt);
                case DependencyTrait dt:
                    return new DependencyTraitStagingArea(dt);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private void OnAdd()
        {
            var param = new DialogParameters
            {
                 { TraitDialogParameters.ExistingTraits, Model.Traits.Select(l => l.Name).ToList() },
                 { TraitDialogParameters.TraitStagingArea, CreateStaging(Model.CreateTrait()) },
            };

            dialogService.ShowDialog(DialogVM.CreateTraitDialog, param, AddCallback);
        }

        private void OnEdit(Trait model)
        {
            var param = new DialogParameters
            {
                 { TraitDialogParameters.ExistingTraits, Model.Traits.Where(l => l.Name != model.Name).Select(l => l.Name).ToList() },
                 { TraitDialogParameters.TraitStagingArea, CreateStaging(model) },
            };

            dialogService.ShowDialog(DialogVM.EditTraitDialog, param, EditCallback);
        }

        private void OnDelete(Trait model)
        {
            var param = new DialogParameters
            {
                 { nameof(TraitDialogParameters.Message), string.Format(CultureInfo.CurrentCulture, Strings.DeleteTraitConfirmation, model.Name) },
                 { nameof(TraitDialogParameters.Index), Model.Traits.IndexOf(model) },
            };

            dialogService.ShowDialog(DialogVM.DeleteTraitDialog, param, DeleteCallback);
        }

        private void AddCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(TraitDialogParameters.Trait, out Trait trait))
            {
                TraitIconVMs.Add(new TraitIconVM(trait, OnEdit, OnDelete));
                sessionProvider.Session().OnTraitsChanged();
            }
        }

        private void EditCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(TraitDialogParameters.Trait, out Trait trait))
            {
                TraitIconVMs.Replace(TraitIconVMs.First(l => l.Model == trait), new TraitIconVM(trait, OnEdit, OnDelete));
                sessionProvider.Session().OnTraitsChanged();
            }
        }

        private void DeleteCallback(IDialogResult dialogResult)
        {
            if (dialogResult.Result == ButtonResult.OK &&
                dialogResult.Parameters.TryGetValue(nameof(DeleteDialogVM.Index), out int index))
            {
                TraitIconVMs.RemoveAt(index);
                sessionProvider.Session().OnTraitsChanged();
            }
        }

        private void OnDrop(DragEventArgs args)
        {
            if (args.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])args.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    string ironUri = string.Empty;

                    FileAttributes attr = File.GetAttributes(file);

                    if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        ironUri = file;
                    }

                    var model = Model.CreateTrait(
                        Path.GetFileNameWithoutExtension(new FileInfo(file).Name),
                        ironUri);

                    switch (model)
                    {
                        case DrawnTrait dt:
                            dt.TraitURI = file;
                            break;
                        case DependencyTrait dt:
                            dt.Variants[0].TraitURI = file;
                            break;
                    }

                    var param = new DialogParameters
                    {
                         { TraitDialogParameters.ExistingTraits, Model.Traits.Select(l => l.Name).ToList() },
                         { TraitDialogParameters.TraitStagingArea, CreateStaging(model) },
                    };

                    dialogService.ShowDialog(DialogVM.CreateTraitDialog, param, AddCallback);
                }
            }
        }
    }
}
