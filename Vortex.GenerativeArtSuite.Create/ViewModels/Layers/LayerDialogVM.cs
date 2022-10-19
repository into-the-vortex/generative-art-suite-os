using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers
{
    public abstract class LayerDialogVM : DialogVM
    {
        public const string ExistingLayerNames = nameof(ExistingLayerNames);

        private List<PathSelector> paths = new();
        private IDisposable? pathLink;

        private List<string> existingLayerNames = new();
        private string name = string.Empty;
        private string path = string.Empty;
        private bool optional;
        private bool includeInDNA = true;
        private bool affectedByLayerMask = true;

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
            UpdatePathSource(new());
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

        public ObservableCollection<PathSelectorVM> Paths { get; } = new();

        public ICommand Confirm { get; }

        public ICommand AddPath { get; }

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

        protected Layer Create() => new(name, optional, includeInDNA, affectedByLayerMask, paths);

        protected void UpdatePathSource(List<PathSelector> source)
        {
            pathLink?.Dispose();
            paths = source;
            pathLink = Paths.ConnectModelCollection(paths, p => new PathSelectorVM(p, RemovePath));
        }

        private void OnAddPath()
        {
            Paths.Add(new PathSelectorVM(new PathSelector(path), RemovePath));
            Path = string.Empty;
        }

        private bool CanAddPath()
        {
            if (path is null)
            {
                return false;
            }

            var possiblePath = new PathSelector(path);
            var existingPaths = Paths.Select(p => p.Model).SelectMany(p => p.Options);

            return possiblePath.Options.Count >= 2 && !existingPaths.Any(p => possiblePath.Options.Contains(p));
        }

        private void RemovePath(PathSelectorVM vm)
        {
            Paths.Remove(vm);
        }
    }
}
