using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Settings
{
    public class SettingsVM : SessionAwareVM
    {
        private readonly IFileSystem fileSystem;
        private SessionSettingsVM settings;

        public SettingsVM(IFileSystem fileSystem, SessionSettings settings)
        {
            this.fileSystem = fileSystem;
            this.settings = new SessionSettingsVM(fileSystem, settings);
        }

        public SessionSettingsVM Settings
        {
            get => settings;
            set => SetProperty(ref settings, value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            Settings = new SessionSettingsVM(fileSystem, Session().Settings);
        }
    }
}
