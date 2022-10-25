using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public abstract class LayerDialogVM : DialogVM
    {
        public const string ExistingLayerNames = nameof(ExistingLayerNames);

        private LayerStagingArea? layerStagingArea;
        private List<string> existingLayerNames = new();
        private string path = string.Empty;

        public LayerDialogVM()
        {
            var confirm = new DelegateCommand(() => CloseDialog(OKAY), CanConfirm);
            var addPath = new DelegateCommand(OnAddPath, CanAddPath);
            PropertyChanged += (s, e) =>
            {
                confirm.RaiseCanExecuteChanged();
                addPath.RaiseCanExecuteChanged();
            };

            Confirm = confirm;
            AddPath = addPath;
            Cancel = new DelegateCommand(() => CloseDialog(CANCEL), CanCloseDialog);
        }

        public string Path
        {
            get => path;
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<PathSelectorVM> PathVMs { get; } = new();

        public ICommand Confirm { get; }

        public ICommand AddPath { get; }

        public ICommand Cancel { get; }

        public string Name
        {
            get => layerStagingArea == null ? string.Empty : layerStagingArea.Name.Value;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.Name.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Optional
        {
            get => layerStagingArea != null && layerStagingArea.Optional.Value;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.Optional.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IncludeInDNA
        {
            get => layerStagingArea != null && layerStagingArea.IncludeInDNA.Value;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.IncludeInDNA.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AffectedByLayerMask
        {
            get => layerStagingArea != null && layerStagingArea.AffectedByLayerMask.Value;
            set
            {
                if (layerStagingArea != null)
                {
                    layerStagingArea.AffectedByLayerMask.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(ExistingLayerNames, out List<string> layerNames))
            {
                existingLayerNames = layerNames;
            }

            if (parameters.TryGetValue(nameof(LayerStagingArea), out LayerStagingArea layerStagingArea))
            {
                this.layerStagingArea = layerStagingArea;
                PathVMs.ConnectModelCollection(layerStagingArea.Paths, m => new PathSelectorVM(m, RemovePath));

                OnPropertyChanged(string.Empty);
            }
        }

        protected override IDialogParameters GetDialogParameters(string parameter)
        {
            var result = new DialogParameters();

            if (parameter == OKAY)
            {
                result.Add(nameof(Layer), layerStagingArea?.Apply());
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
            return !existingLayerNames.Contains(Name) &&
                !string.IsNullOrWhiteSpace(Name) &&
                layerStagingArea?.IsDirty() == true;
        }

        private void OnAddPath()
        {
            PathVMs.Add(new PathSelectorVM(new PathSelector(path), RemovePath));
            Path = string.Empty;
        }

        private bool CanAddPath()
        {
            if (path is null)
            {
                return false;
            }

            var possiblePath = new PathSelector(path);
            var existingPaths = PathVMs.Select(p => p.Model).SelectMany(p => p.Options);

            return possiblePath.Options.Count >= 2 && !existingPaths.Any(p => possiblePath.Options.Contains(p));
        }

        private void RemovePath(PathSelectorVM vm)
        {
            PathVMs.Remove(vm);
            OnPropertyChanged(nameof(Confirm));
        }
    }
}
