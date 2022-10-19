using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Home
{
    public class SessionVM : SessionAwareVM
    {
        private readonly INavigationService navigationService;
        private readonly IFileSystem fileSystem;
        private string? selectedTag;

        public SessionVM(IFileSystem fileSystem, INavigationService navigationService)
        {
            this.fileSystem = fileSystem;
            this.navigationService = navigationService;
        }

        public string? SelectedTag
        {
            get => selectedTag;
            set => OnSelectedTagChanged(value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            SelectedTag = NavigationService.Layers;
        }

        private void OnSelectedTagChanged(string? tag)
        {
            if (tag is not null)
            {
                selectedTag = tag;
                OnPropertyChanged(nameof(SelectedTag));

                switch (tag)
                {
                    case NavigationService.Home:
                        // TODO: Probably dialog this to make sure they want to save.
                        fileSystem.SaveSession(Session());
                        navigationService.GoHome();
                        break;
                    default:
                        var parameters = new NavigationParameters
                        {
                            { nameof(Session), Session() },
                        };
                        navigationService.NavigateTo(tag, parameters);
                        break;
                }
            }
        }
    }
}
