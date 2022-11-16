using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models.Traits;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Base
{
    public abstract class TraitDialogVM : DialogVM
    {
        private readonly Action raiseCanExecuteChanged;
        private readonly IFileSystem fileSystem;

        private List<string> existingTraitNames = new();
        private TraitStagingArea? traitStagingArea;
        private TraitVM? traitVM;

        public TraitDialogVM(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            var confirm = new DelegateCommand(() => CloseDialog(OKAY), CanConfirm);
            raiseCanExecuteChanged = new Action(confirm.RaiseCanExecuteChanged);

            Confirm = confirm;
            Cancel = new DelegateCommand(() => CloseDialog(CANCEL));
        }

        public TraitVM? TraitVM
        {
            get => traitVM;
            set => SetProperty(ref traitVM, value);
        }

        public ICommand Confirm { get; }

        public ICommand Cancel { get; }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(TraitDialogParameters.ExistingTraits, out List<string> traitNames))
            {
                existingTraitNames = traitNames;
            }

            if (parameters.TryGetValue(TraitDialogParameters.TraitStagingArea, out TraitStagingArea traitStagingArea))
            {
                this.traitStagingArea = traitStagingArea;
                TraitVM = CreateVM(traitStagingArea);
            }
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            var result = new DialogParameters();

            if (parameter == OKAY)
            {
                result.Add(TraitDialogParameters.Trait, traitStagingArea?.Apply());
            }

            return result;
        }

        protected override ButtonResult GetButtonResult(string parameter)
        {
            switch (parameter)
            {
                case OKAY:
                    return ButtonResult.OK;
                case CANCEL:
                    return ButtonResult.Cancel;
                default:
                    return ButtonResult.None;
            }
        }

        protected virtual bool CanConfirm()
        {
            return TraitVM?.Name != NoneTrait.NAME &&
                !existingTraitNames.Any(name => name == TraitVM?.Name) &&
                !string.IsNullOrWhiteSpace(TraitVM?.Name) &&
                TraitVM?.CanConfirm() == true;
        }

        private TraitVM CreateVM(TraitStagingArea stagingArea)
        {
            switch (stagingArea)
            {
                case PathingTraitStagingArea pt:
                    return new PathingTraitVM(fileSystem, pt, raiseCanExecuteChanged);
                case DrawnTraitStagingArea dt:
                    return new DrawnTraitVM(fileSystem, dt, raiseCanExecuteChanged);
                case DependencyTraitStagingArea dt:
                    return new DependencyTraitVM(fileSystem, dt, raiseCanExecuteChanged);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
