using Prism.Regions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class SessionVM : NotifyPropertyChanged, INavigationAware
    {
        private readonly INavigationService navigationService;
        private readonly IFileSystem fileSystem;
        private Session? currentSession;
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

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SelectedTag = NavigationService.Layers;
            if (navigationContext.Parameters[nameof(Session)] is Session session)
            {
                currentSession = session;
            }
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
                        if (currentSession is not null)
                        {
                            // TODO: Probably dialog this to make sure they want to save.
                            fileSystem.SaveSession(currentSession);
                        }

                        navigationService.GoHome();
                        break;
                    default:
                        var parameters = new NavigationParameters
                        {
                            { nameof(Session), currentSession },
                        };
                        navigationService.NavigateTo(tag, parameters);
                        break;
                }
            }
        }
    }
}
