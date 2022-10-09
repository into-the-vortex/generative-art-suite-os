using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class SettingsVM : SessionAwareVM
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

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            Settings = new SessionSettingsVM(Session().Settings);
        }
    }
}
