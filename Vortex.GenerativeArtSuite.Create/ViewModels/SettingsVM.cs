using Prism.Regions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class SettingsVM : NotifyPropertyChanged, INavigationAware
    {
        private SessionSettingsVM settings;

        public SettingsVM(SessionSettings settings)
        {
            this.settings = new SessionSettingsVM(settings);
        }

        public SessionSettingsVM Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters[nameof(Session)] is Session session)
            {
                Settings = new SessionSettingsVM(session.Settings);
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
    }
}
