using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitsVM : SessionAwareVM
    {
        private object content;
        private string? selectedLayer;

        public TraitsVM()
        {
            content = new NoLayersVM();

            Add = new DelegateCommand(() => (Content as LayerTraitVM)?.Add(), CanAdd);
            MultiAdd = new DelegateCommand(() => (Content as LayerTraitVM)?.MultiAdd(), CanAdd);
        }

        public ObservableCollection<string> Layers { get; } = new();

        public string? SelectedLayer
        {
            get => selectedLayer;
            set
            {
                if (selectedLayer != value)
                {
                    selectedLayer = value;
                    OnPropertyChanged();
                    UpdateContent();
                }
            }
        }

        public object Content
        {
            get => content;
            set
            {
                if (content != value)
                {
                    content = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand Add { get; }

        public DelegateCommand MultiAdd { get; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            Layers.Clear();
            Layers.AddRange(Session().Layers.Select(l => l.Name));
            OnPropertyChanged(nameof(Layers));

            SelectedLayer = Layers.FirstOrDefault();
        }

        private void UpdateContent()
        {
            if (selectedLayer is null)
            {
                Content = new NoLayersVM();
                return;
            }

            var layer = Session().Layers.First(l => l.Name == selectedLayer);

            if (layer is null)
            {
                throw new InvalidOperationException();
            }

            Content = new LayerTraitVM(layer);

            Add.RaiseCanExecuteChanged();
            MultiAdd.RaiseCanExecuteChanged();
        }

        private bool CanAdd()
        {
            return Content is LayerTraitVM;
        }
    }
}
