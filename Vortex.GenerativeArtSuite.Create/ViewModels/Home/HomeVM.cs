using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Home
{
    public class HomeVM : NavigationAwareVM
    {
        private readonly IFileSystem fileSystem;
        private readonly ISessionManager sessionManager;
        private readonly INavigationLock navigationLock;
        private readonly INavigationService navigationService;

        public HomeVM(IFileSystem fileSystem, ISessionManager sessionManager, INavigationLock navigationLock, INavigationService navigationService)
        {
            this.fileSystem = fileSystem;
            this.sessionManager = sessionManager;
            this.navigationLock = navigationLock;
            this.navigationService = navigationService;

            NewSession = new NewSessionVM(fileSystem, sessionManager, navigationService, NameIsValid);
            RecentSessions = new ObservableCollection<RecentSessionVM>();
        }

        public NewSessionVM NewSession { get; }

        public ObservableCollection<RecentSessionVM> RecentSessions { get; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            NewSession.Clear();
            RecentSessions.Clear();
            RecentSessions.AddRange(fileSystem.RecentSessions().Select(s => new RecentSessionVM(s, sessionManager, navigationService)));

            navigationLock.Capture();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
            navigationLock.Release();
        }

        private bool NameIsValid(string name)
        {
            if (RecentSessions.Any(rs => rs.Name == name))
            {
                return false;
            }

            // Windows file system naming regex.
            var rg = new Regex(@"^(?!(?:CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])(?:\.[^.]*)?$)[^<>:""\/\\|?*\x00-\x1F]*[^<>:""\/\\|?*\x00-\x1F\ .]$");
            return rg.IsMatch(name);
        }
    }
}
