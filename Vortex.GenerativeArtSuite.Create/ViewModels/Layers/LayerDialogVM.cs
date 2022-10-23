using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public abstract class LayerDialogVM : DialogVM
    {
        public const string ExistingLayerNames = nameof(ExistingLayerNames);

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

        public abstract string Name { get; set; }

        public abstract bool Optional { get; set; }

        public abstract bool IncludeInDNA { get; set; }

        public abstract bool AffectedByLayerMask { get; set; }

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

        protected void RemovePath(PathSelectorVM vm)
        {
            PathVMs.Remove(vm);
            OnPropertyChanged(nameof(Confirm));
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
    }
}
