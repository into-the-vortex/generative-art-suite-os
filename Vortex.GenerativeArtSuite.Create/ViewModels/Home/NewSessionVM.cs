using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Settings;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Home
{
    public class NewSessionVM : BindableBase
    {
        private readonly Func<string, bool> validateName;
        private readonly IFileSystem fileSystem;
        private readonly ISessionManager sessionManager;
        private readonly INavigationService navigationService;

        private GenerationSettingsVM generationSettings;
        private UserSettingsVM userSettings;

        private bool nameLock;
        private bool clone;
        private bool busy;

        private string buttonCTA;
        private string remote;
        private string name;

        public NewSessionVM(
            IFileSystem fileSystem,
            ISessionManager sessionManager,
            INavigationService navigationService,
            Func<string, bool> validateName)
        {
            this.fileSystem = fileSystem;
            this.sessionManager = sessionManager;
            this.navigationService = navigationService;
            this.validateName = validateName;

            generationSettings = new GenerationSettingsVM(new());
            userSettings = new UserSettingsVM(fileSystem, new());
            buttonCTA = Strings.CreateSession;
            remote = string.Empty;
            name = string.Empty;

            sessionManager.OnBusyChanged += busy =>
            {
                Busy = busy;
            };

            NewSessionCommand = new DelegateCommand(OnNewSession, CanCreateNewSession)
                .ObservesProperty(() => Name)
                .ObservesProperty(() => Busy)
                .ObservesProperty(() => Clone)
                .ObservesProperty(() => Remote);
        }

        public bool Busy
        {
            get => busy;
            set => SetProperty(ref busy, value, UpdateCTA);
        }

        public bool Clone
        {
            get => clone;
            set => SetProperty(ref clone, value, UpdateCTA);
        }

        public bool NameLock
        {
            get => nameLock;
            set => SetProperty(ref nameLock, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Remote
        {
            get => remote;
            set => SetProperty(ref remote, value, CheckForNameLock);
        }

        public string ButtonCTA
        {
            get => buttonCTA;
            set => SetProperty(ref buttonCTA, value, CheckForNameLock);
        }

        public UserSettingsVM UserSettingsVM
        {
            get => userSettings;
            set => SetProperty(ref userSettings, value, () => HookupEvents(UserSettingsVM));
        }

        public GenerationSettingsVM GenerationSettingsVM
        {
            get => generationSettings;
            set => SetProperty(ref generationSettings, value, () => HookupEvents(GenerationSettingsVM));
        }

        public DelegateCommand NewSessionCommand { get; }

        public void Clear()
        {
            Clone = false;
            Name = string.Empty;
            Remote = string.Empty;
            UserSettingsVM = new UserSettingsVM(fileSystem, new());
            GenerationSettingsVM = new GenerationSettingsVM(new());
        }

        private void OnNewSession()
        {
            Task.Run(async () =>
            {
                try
                {
                    if (clone)
                    {
                        await sessionManager.CloneNewSession(name, remote, userSettings.Model);
                    }
                    else
                    {
                        await sessionManager.CreateNewSession(remote, new Session(name, userSettings.Model, generationSettings.Model));
                    }

                    navigationService.NavigateTo(NavigationService.Layers);
                }
                catch (Exception e)
                {
                    fileSystem.DeleteSession(name);
                    MessageBox.Show(e.Message);
                    Clear();
                }
            });
        }

        private bool CanCreateNewSession()
        {
            return !busy &&
                validateName(name) &&
                userSettings.IsValid() &&
                (clone || generationSettings.IsValid());
        }

        private void HookupEvents(BindableBase bb)
        {
            bb.PropertyChanged += (s, e) =>
            {
                NewSessionCommand.RaiseCanExecuteChanged();
            };
        }

        private void UpdateCTA()
        {
            ButtonCTA = busy
                ? Strings.Syncing
                : (clone ? Strings.CloneSession : Strings.CreateSession);
        }

        private void CheckForNameLock()
        {
            if (string.IsNullOrWhiteSpace(remote))
            {
                NameLock = false;
                return;
            }

            var info = new FileInfo(Remote);
            NameLock = info.Extension == ".git";

            if (NameLock)
            {
                Name = Path.GetFileNameWithoutExtension(info.FullName);
            }
        }
    }
}
