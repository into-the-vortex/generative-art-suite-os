using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class CreateLayerDialogVM : DialogVM
    {
        public const string ExistingLayerNames = nameof(ExistingLayerNames);

        private List<string> existingLayerNames = new();
        private int index;
        private int maxIndex;
        private string name = string.Empty;
        private bool optional;
        private bool includeInDNA = true;

        public CreateLayerDialogVM()
        {
            var create = new DelegateCommand(() => CloseDialog(OKAY), CanAdd);
            PropertyChanged += (s, e) =>
            {
                create.RaiseCanExecuteChanged();
            };

            Confirm = create;
            Cancel = new DelegateCommand(() => CloseDialog(CANCEL), CanCloseDialog);
        }

        public override string Title => Strings.CreateLayer;

        public int Index
        {
            get => index;
            set
            {
                if (index != value && index >= 0 && index <= maxIndex)
                {
                    index = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxIndex
        {
            get => maxIndex;
            set
            {
                if (maxIndex != value)
                {
                    maxIndex = value;
                    OnPropertyChanged();
                }
            }
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

        public ICommand Confirm { get; }

        public ICommand Cancel { get; }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(ExistingLayerNames, out List<string> layerNames))
            {
                MaxIndex = layerNames.Count;
                Index = MaxIndex;
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

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            var result = new DialogParameters();

            if (parameter == OKAY)
            {
                result.Add(nameof(Layer), new Layer(Index, Name, optional, includeInDNA));
            }

            return result;
        }

        private bool CanAdd()
        {
            return !existingLayerNames.Contains(Name) &&
                !string.IsNullOrWhiteSpace(Name) &&
                Index >= 0 &&
                Index <= MaxIndex;
        }
    }
}
