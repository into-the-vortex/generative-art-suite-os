﻿using System.Collections.ObjectModel;
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
        private TraitsVM? content;
        private string? selectedLayer;

        public LayerSelectorVM(IDialogService dialogService)
        {
            this.dialogService = dialogService;
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
            set => SetProperty(ref content, value, () => RaisePropertyChanged(nameof(Add)));
        }

        public ICommand? Add => Content is TraitsVM traitsVM ? traitsVM.Add : null;

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
            Content = layer is not null ? new TraitsVM(dialogService, layer) : null;
        }
    }
}
