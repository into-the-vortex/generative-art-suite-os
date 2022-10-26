﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public abstract class TraitDialogVM : DialogVM
    {
        public const string ExistingTraitNames = nameof(ExistingTraitNames);

        private TraitStagingArea? traitStagingArea;
        private List<string> existingTraitNames = new();

        public TraitDialogVM()
        {
            var confirm = new DelegateCommand(() => CloseDialog(OKAY), CanConfirm);

            VariantVMs.CollectionChanged += (s, e) =>
            {
                confirm.RaiseCanExecuteChanged();
            };

            PropertyChanged += (s, e) =>
            {
                confirm.RaiseCanExecuteChanged();
            };

            Confirm = confirm;
            BrowseIcon = new DelegateCommand(OnBrowseIcon);
            Cancel = new DelegateCommand(() => CloseDialog(CANCEL), CanCloseDialog);
        }

        public ICommand Confirm { get; }

        public ICommand BrowseIcon { get; }

        public ICommand Cancel { get; }

        public string WeightLabel => $"{Strings.Weight} - {Weight:D3}";

        public string Name
        {
            get => traitStagingArea?.Name.Value ?? string.Empty;
            set
            {
                if (traitStagingArea != null)
                {
                    traitStagingArea.Name.Value = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string IconURI
        {
            get => traitStagingArea?.IconURI.Value ?? string.Empty;
            set
            {
                if (traitStagingArea != null)
                {
                    traitStagingArea.IconURI.Value = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int Weight
        {
            get => traitStagingArea?.Weight.Value ?? 0;
            set
            {
                if (traitStagingArea != null)
                {
                    traitStagingArea.Weight.Value = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(WeightLabel));
                }
            }
        }

        public ObservableCollection<TraitVariantVM> VariantVMs { get; } = new();

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(ExistingTraitNames, out List<string> traitNames))
            {
                existingTraitNames = traitNames;
            }

            if (parameters.TryGetValue(nameof(TraitStagingArea), out TraitStagingArea traitStagingArea))
            {
                this.traitStagingArea = traitStagingArea;
                VariantVMs.ConnectModelCollection(traitStagingArea.Variants, m => new TraitVariantVM(m));

                RaisePropertyChanged(string.Empty);
            }
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            var result = new DialogParameters();

            if (parameter == OKAY)
            {
                result.Add(nameof(Trait), traitStagingArea?.Apply());
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
            return Name != Trait.NONENAME &&
                !existingTraitNames.Contains(Name) &&
                !string.IsNullOrWhiteSpace(Name) &&
                File.Exists(IconURI) &&
                VariantVMs.All(v => File.Exists(v.ImagePath));
        }

        private void OnBrowseIcon()
        {
        }
    }
}