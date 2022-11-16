using System.Collections.ObjectModel;
using System.Linq;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class LayerSelectorVM : SessionAwareVM
    {
        private readonly IDialogService dialogService;
        private readonly ISessionProvider sessionProvider;
        private LayerTraitsVM? content;
        private string? selectedLayer;

        public LayerSelectorVM(IFileSystem fileSystem, IDialogService dialogService, ISessionProvider sessionProvider)
            : base(sessionProvider, dialogService)
        {
            this.dialogService = dialogService;
            this.sessionProvider = sessionProvider;
        }

        public ObservableCollection<string> Layers { get; } = new();

        public string? SelectedLayer
        {
            get => selectedLayer;
            set => SetProperty(ref selectedLayer, value, UpdateContent);
        }

        public LayerTraitsVM? Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }

        protected override void ResetOnSessionChanged()
        {
            Layers.Clear();
            Layers.AddRange(sessionProvider.Session().Layers.Select(l => l.Name));
            RaisePropertyChanged(nameof(Layers));

            SelectedLayer = Layers.FirstOrDefault();
        }

        private void UpdateContent()
        {
            var layer = sessionProvider.Session().Layers.FirstOrDefault(l => l.Name == selectedLayer);
            Content = layer is not null ? new LayerTraitsVM(sessionProvider, dialogService, layer) : null;
        }
    }
}
