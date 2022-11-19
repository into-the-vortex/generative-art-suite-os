using System;
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

        private GenerationSettingsVM generationSettings;
        private UserSettingsVM userSettings;
        private string remote;
        private string name;

        public NewSessionVM(IFileSystem fileSystem, Func<string, bool> validateName, Action<string, Session> onCreate)
        {
            this.fileSystem = fileSystem;
            this.validateName = validateName;

            generationSettings = new GenerationSettingsVM(new());
            userSettings = new UserSettingsVM(fileSystem, new());
            remote = string.Empty;
            name = string.Empty;

            Create = new DelegateCommand(() => onCreate(remote, new Session(Name, userSettings.Model, generationSettings.Model)), CanCreate)
                .ObservesProperty(() => Name)
                .ObservesProperty(() => Remote);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Remote
        {
            get => remote;
            set => SetProperty(ref remote, value);
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

        public DelegateCommand Create { get; }

        public void Clear()
        {
            Name = string.Empty;
            Remote = string.Empty;
            UserSettingsVM = new UserSettingsVM(fileSystem, new());
            GenerationSettingsVM = new GenerationSettingsVM(new());
        }

        private void HookupEvents(BindableBase bb)
        {
            bb.PropertyChanged += (s, e) =>
            {
                Create.RaiseCanExecuteChanged();
            };
        }

        private bool CanCreate()
        {
            return validateName(name) &&
                userSettings.IsValid() &&
                generationSettings.IsValid();
        }
    }
}
