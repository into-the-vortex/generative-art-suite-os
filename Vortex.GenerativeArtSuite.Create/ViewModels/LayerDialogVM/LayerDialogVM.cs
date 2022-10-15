using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.LayerDialogVM
{
    public abstract class LayerDialogVM : DialogVM
    {
        public const string ExistingLayerNames = nameof(ExistingLayerNames);

        private List<string> existingLayerNames = new();
        private string name = string.Empty;
        private bool optional;
        private bool includeInDNA = true;
        private bool affectedByLayerMask = true;

        public LayerDialogVM()
        {
            var confirm = new DelegateCommand(() => CloseDialog(OKAY), CanConfirm);
            PropertyChanged += (s, e) =>
            {
                confirm.RaiseCanExecuteChanged();
            };

            Confirm = confirm;
            Cancel = new DelegateCommand(() => CloseDialog(CANCEL), CanCloseDialog);
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Optional
        {
            get => optional;
            set
            {
                if (optional != value)
                {
                    optional = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IncludeInDNA
        {
            get => includeInDNA;
            set
            {
                if (includeInDNA != value)
                {
                    includeInDNA = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AffectedByLayerMask
        {
            get => affectedByLayerMask;
            set
            {
                if (affectedByLayerMask != value)
                {
                    affectedByLayerMask = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand Confirm { get; }

        public ICommand Cancel { get; }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(ExistingLayerNames, out List<string> layerNames))
            {
                existingLayerNames = layerNames;
            }
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
            return !existingLayerNames.Contains(Name) &&
                !string.IsNullOrWhiteSpace(Name);
        }

        protected Layer Create() => new(name, optional, includeInDNA, affectedByLayerMask);
    }
}
