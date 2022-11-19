using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Settings
{
    public class SettingsVM : SessionAwareVM
    {
        private readonly ISessionProvider sessionProvider;
        private readonly IFileSystem fileSystem;

        private GenerationSettingsVM? generationSettings;
        private UserSettingsVM? userSettings;

        public SettingsVM(ISessionProvider sessionProvider, IDialogService dialogService, IFileSystem fileSystem)
            : base(sessionProvider, dialogService)
        {
            this.sessionProvider = sessionProvider;
            this.fileSystem = fileSystem;
        }

        public UserSettingsVM? UserSettings
        {
            get => userSettings;
            set => SetProperty(ref userSettings, value);
        }

        public GenerationSettingsVM? GenerationSettings
        {
            get => generationSettings;
            set => SetProperty(ref generationSettings, value);
        }

        protected override void ResetOnSessionChanged()
        {
            UserSettings = new UserSettingsVM(fileSystem, sessionProvider.Session().UserSettings);
            GenerationSettings = new GenerationSettingsVM(sessionProvider.Session().GenerationSettings);
        }
    }
}
