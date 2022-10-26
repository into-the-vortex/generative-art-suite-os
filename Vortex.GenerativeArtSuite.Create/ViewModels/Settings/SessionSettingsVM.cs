using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Settings
{
    public class SessionSettingsVM : BindableBase
    {
        private readonly IFileSystem fileSystem;

        public SessionSettingsVM(IFileSystem fileSystem, SessionSettings settings)
        {
            this.fileSystem = fileSystem;
            Settings = settings;
            BrowseOutputFolder = new DelegateCommand(OnBrowseOutputFolder);
        }

        public string OutputFolder
        {
            get => Settings.OutputFolder;
            set
            {
                if (Settings.OutputFolder != value)
                {
                    Settings.OutputFolder = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string NamePrefix
        {
            get => Settings.NamePrefix;
            set
            {
                if (Settings.NamePrefix != value)
                {
                    Settings.NamePrefix = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string DescriptionTemplate
        {
            get => Settings.DescriptionTemplate;
            set
            {
                if (Settings.DescriptionTemplate != value)
                {
                    Settings.DescriptionTemplate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string BaseURI
        {
            get => Settings.BaseURI;
            set
            {
                if (Settings.BaseURI != value)
                {
                    Settings.BaseURI = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int CollectionSize
        {
            get => Settings.CollectionSize;
            set
            {
                if (Settings.CollectionSize != value)
                {
                    Settings.CollectionSize = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand BrowseOutputFolder { get; }

        protected SessionSettings Settings { get; }

        private void OnBrowseOutputFolder()
        {
            OutputFolder = fileSystem.SelectFolder();
        }
    }
}
