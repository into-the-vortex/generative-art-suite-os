using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class EditLayerDialogVM : DialogVM
    {
        public const string OldLayer = nameof(OldLayer);
        public const string NewLayer = nameof(NewLayer);
        public const string ExistingLayerNames = nameof(ExistingLayerNames);

        private List<string> existingLayerNames = new();
        private Layer? oldLayer;
        private int index;
        private int maxIndex;
        private string name = string.Empty;
        private bool optional;
        private bool includeInDNA = true;

        public EditLayerDialogVM()
        {
            var edit = new DelegateCommand(() => CloseDialog(OKAY), CanEdit);
            PropertyChanged += (s, e) =>
            {
                edit.RaiseCanExecuteChanged();
            };

            Confirm = edit;
            Cancel = new DelegateCommand(() => CloseDialog(CANCEL), CanCloseDialog);
        }

        public override string Title => Strings.EditLayer;

        public int Index
        {
            get => index;
            set
            {
                if (index != value && value >= 0 && value <= maxIndex)
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
                existingLayerNames = layerNames;
            }

            if (parameters.TryGetValue(nameof(Layer), out Layer layer))
            {
                oldLayer = layer;
                Name = layer.Name;
                Optional = layer.Optional;
                IncludeInDNA = layer.IncludeInDNA;
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

            if (parameter == OKAY && oldLayer != null)
            {
                result.Add(NewLayer, new Layer(Name, optional, includeInDNA));
                result.Add(OldLayer, oldLayer);
            }

            return result;
        }

        private bool CanEdit()
        {
            return !existingLayerNames.Contains(Name) &&
                !string.IsNullOrWhiteSpace(Name) &&
                Index >= 0 &&
                Index <= MaxIndex;
        }
    }
}
