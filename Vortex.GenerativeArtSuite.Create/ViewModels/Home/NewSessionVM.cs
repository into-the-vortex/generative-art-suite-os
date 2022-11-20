using System;
using System.IO;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Settings;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Home
{
    public class NewSessionVM : BindableBase
    {
        private readonly Action<string, string, UserSettings> onClone;
        private readonly Action<string, Session> onCreate;
        private readonly Func<string, bool> validateName;
        private readonly IFileSystem fileSystem;

        private GenerationSettingsVM generationSettings;
        private UserSettingsVM userSettings;
        private bool nameLock;
        private bool clone;
        private string buttonCTA;
        private string remote;
        private string name;

        public NewSessionVM(
            IFileSystem fileSystem,
            Func<string, bool> validateName,
            Action<string, Session> onCreate,
            Action<string, string, UserSettings> onClone)
        {
            this.fileSystem = fileSystem;
            this.onClone = onClone;
            this.onCreate = onCreate;
            this.validateName = validateName;

            generationSettings = new GenerationSettingsVM(new());
            userSettings = new UserSettingsVM(fileSystem, new());
            buttonCTA = Strings.CreateSession;
            remote = string.Empty;
            name = string.Empty;

            NewSessionCommand = new DelegateCommand(OnNewSession, CanCreateNewSession)
                .ObservesProperty(() => Name)
                .ObservesProperty(() => Clone)
                .ObservesProperty(() => Remote);
        }

        public bool Clone
        {
            get => clone;
            set => SetProperty(ref clone, value, OnCloneChanged);
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
            if (clone)
            {
                onClone(name, remote, userSettings.Model);
            }
            else
            {
                onCreate(remote, new Session(Name, userSettings.Model, generationSettings.Model));
            }
        }

        private bool CanCreateNewSession()
        {
            return validateName(name) &&
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

        private void OnCloneChanged()
        {
            ButtonCTA = clone
                ? Strings.CloneSession
                : Strings.CreateSession;
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
