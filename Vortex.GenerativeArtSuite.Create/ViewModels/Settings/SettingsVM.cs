using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Settings
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
