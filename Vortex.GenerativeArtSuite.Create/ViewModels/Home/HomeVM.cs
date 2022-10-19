using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Home
{
    public class HomeVM : NotifyPropertyChanged
    {
        private readonly IFileSystem fileSystem;
        private readonly INavigationService navigationService;

        public HomeVM(IFileSystem fileSystem, INavigationService navigationService)
        {
            this.fileSystem = fileSystem;
            this.navigationService = navigationService;

            NewSession = new NewSessionVM(NameIsValid, OpenNewSession);
            RecentSessions = new List<RecentSessionVM>(fileSystem.RecentSessions().Select(s => new RecentSessionVM(s, OpenRecentSession)));
        }

        public NewSessionVM NewSession { get; }

        public IEnumerable<RecentSessionVM> RecentSessions { get; }

        private void OpenNewSession(string name, SessionSettings sessionSettings)
        {
            var session = new Session(name, sessionSettings);
            fileSystem.SaveSession(session);
            navigationService.OpenSession(session);
        }

        private void OpenRecentSession(string name)
        {
            var session = fileSystem.LoadSession(name);
            navigationService.OpenSession(session);
        }

        private bool NameIsValid(string name)
        {
            if (RecentSessions.Any(rs => rs.Name == name))
            {
                return false;
            }

            var rg = new Regex(@"^(?!(?:CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])(?:\.[^.]*)?$)[^<>:""\/\\|?*\x00-\x1F]*[^<>:""\/\\|?*\x00-\x1F\ .]$");
            return rg.IsMatch(name);
        }
    }
}
