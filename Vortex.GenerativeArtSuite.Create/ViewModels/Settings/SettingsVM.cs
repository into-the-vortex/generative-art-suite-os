using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Settings
{
    public class SettingsVM : SessionAwareVM
    {
        private readonly ISessionProvider sessionProvider;
        private readonly IFileSystem fileSystem;
        private SessionSettingsVM settings;

        public SettingsVM(ISessionProvider sessionProvider, IFileSystem fileSystem, SessionSettings settings)
            : base(sessionProvider)
        {
            this.sessionProvider = sessionProvider;
            this.fileSystem = fileSystem;
            this.settings = new SessionSettingsVM(fileSystem, settings);
        }

        public SessionSettingsVM Settings
        {
            get => settings;
            set => SetProperty(ref settings, value);
        }

        protected override void ResetOnSessionChanged()
        {
            Settings = new SessionSettingsVM(fileSystem, sessionProvider.Session().Settings);
        }
    }
}
