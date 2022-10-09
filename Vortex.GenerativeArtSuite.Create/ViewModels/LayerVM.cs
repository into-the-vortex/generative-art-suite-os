using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class LayerVM
    {
        private readonly Layer model;
        private readonly IDialogService dialogService;
        private readonly Action<IDialogResult> editCallback;
        private readonly Action<IDialogResult> deleteCallback;

        public LayerVM(Layer model, IDialogService dialogService, Action<IDialogResult> editCallback, Action<IDialogResult> deleteCallback)
        {
            this.model = model;
            this.dialogService = dialogService;
            this.editCallback = editCallback;
            this.deleteCallback = deleteCallback;

            Fork = new DelegateCommand(OnAddSubLayer);
            Edit = new DelegateCommand(OnEdit);
            Delete = new DelegateCommand(OnDelete);
            BranchVMs = new ObservableCollection<SubLayerVM>(model.Branches.Select(b => new SubLayerVM(b)));
        }

        public string Name => $"{Strings.NameLabel} {model.Name}";

        public string Info => $" - ({(model.Optional ? Strings.IsOptionalOn : Strings.IsOptionalOff)} | {(model.IncludeInDNA ? Strings.IsDNAOff : Strings.IsOptionalOn)})";

        public ObservableCollection<SubLayerVM> BranchVMs { get; }

        public ICommand Fork { get; }

        public ICommand Edit { get; }

        public ICommand Delete { get; }

        private void OnAddSubLayer()
        {
        }

        private void OnEdit()
        {
        }

        private void OnDelete()
        {
            var param = new DialogParameters
            {
                { nameof(DeleteLayerDialogVM.Message), string.Format(CultureInfo.CurrentCulture, Strings.DeleteLayerConfirmation, model.Name) },
                { nameof(Layer), model },
            };

            dialogService.ShowDialog(DialogVM.DeleteLayerDialog, param, deleteCallback);
        }
    }
}
