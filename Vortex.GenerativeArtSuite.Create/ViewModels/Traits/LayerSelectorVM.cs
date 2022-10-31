using System.Collections.ObjectModel;
using System.Linq;
using Prism.Regions;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class LayerSelectorVM : SessionAwareVM
    {
        private readonly IDialogService dialogService;
        private readonly IFileSystem fileSystem;
        private TraitsVM? content;
        private string? selectedLayer;

        public LayerSelectorVM(IFileSystem fileSystem, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            this.fileSystem = fileSystem;
        }

        public ObservableCollection<string> Layers { get; } = new();

        public string? SelectedLayer
        {
            get => selectedLayer;
            set => SetProperty(ref selectedLayer, value, UpdateContent);
        }

        public TraitsVM? Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            Layers.Clear();
            Layers.AddRange(Session().Layers.Select(l => l.Name));
            RaisePropertyChanged(nameof(Layers));

            SelectedLayer = Layers.FirstOrDefault();
        }

        private void UpdateContent()
        {
            var layer = Session().Layers.FirstOrDefault(l => l.Name == selectedLayer);
            Content = layer is not null ? new TraitsVM(fileSystem, dialogService, layer) : null;
        }
    }
}
