using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Prism.Regions;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class LayerSelectorVM : SessionAwareVM
    {
        private readonly IDialogService dialogService;
        private object content;
        private string? selectedLayer;

        public LayerSelectorVM(IDialogService dialogService)
        {
            content = new NoLayersVM();
            this.dialogService = dialogService;
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
                    OnPropertyChanged(nameof(Add));
                }
            }
        }

        public ICommand? Add => Content is TraitsVM traitsVM ? traitsVM.Add : null;

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

            Content = new TraitsVM(dialogService, layer);
        }
    }
}
