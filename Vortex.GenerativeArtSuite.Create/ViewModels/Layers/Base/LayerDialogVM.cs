using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DynamicData;
using Prism.Commands;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models.Layers;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Base
{
    public abstract class LayerDialogVM : DialogVM
    {
        private List<string> existingLayers = new();
        private List<Layer> previousLayers = new();

        private bool canHaveDependencies;
        private string? selectedDependency;

        public LayerDialogVM()
        {
            var add = new DelegateCommand(OnAdd, CanAdd);
            var create = new DelegateCommand(() => CloseDialog(OKAY), CanConfirm);
            var cancel = new DelegateCommand(() => CloseDialog(CANCEL), CanCloseDialog);
            PropertyChanged += (s, e) =>
            {
                add.RaiseCanExecuteChanged();
                create.RaiseCanExecuteChanged();
                cancel.RaiseCanExecuteChanged();
            };
            Add = add;
            Confirm = create;
            Cancel = cancel;
        }

        public abstract string Name { get; set; }

        public abstract bool IncludeInDNA { get; set; }

        public abstract bool Drawn { get; set; }

        public string? SelectedDependency
        {
            get => selectedDependency;
            set => SetProperty(ref selectedDependency, value);
        }

        public bool CanHaveDependencies
        {
            get => canHaveDependencies;
            set => SetProperty(ref canHaveDependencies, value);
        }

        public ICommand Add { get; }

        public ICommand Confirm { get; }

        public ICommand Cancel { get; }

        public ObservableCollection<DependencyVM> Dependencies { get; } = new();

        public ObservableCollection<string> PossibleDependencies { get; } = new();

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(LayerDialogParameters.ExistingLayers, out List<string> existingLayers))
            {
                this.existingLayers = existingLayers;
            }

            if (parameters.TryGetValue(LayerDialogParameters.PreviousLayers, out List<Layer> previousLayers))
            {
                this.previousLayers = previousLayers;
                ResetDependencySelection();
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
            return !existingLayers.Contains(Name) &&
                !string.IsNullOrWhiteSpace(Name);
        }

        protected DependencyVM CreateDependencyVM(Dependency model)
        {
            return new DependencyVM(model, OnRemove);
        }

        private void OnAdd()
        {
            Dependencies.Add(
                CreateDependencyVM(
                    new Dependency(previousLayers.First(l => l.Name == selectedDependency))));

            ResetDependencySelection();
        }

        private bool CanAdd()
        {
            return !string.IsNullOrEmpty(SelectedDependency);
        }

        private void OnRemove(string dependency)
        {
            Dependencies.Remove(Dependencies.First(d => d.Name == dependency));
            ResetDependencySelection();
        }

        private void ResetDependencySelection()
        {
            PossibleDependencies.Clear();
            PossibleDependencies.AddRange(previousLayers.Where(pl => !Dependencies.Any(dp => dp.Name == pl.Name)).Select(pl => pl.Name));
            RaisePropertyChanged(nameof(PossibleDependencies));

            CanHaveDependencies = Dependencies.Any() || PossibleDependencies.Any();
            SelectedDependency = PossibleDependencies.FirstOrDefault();
        }
    }
}
