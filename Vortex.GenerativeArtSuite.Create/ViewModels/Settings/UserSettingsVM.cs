using System.IO;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models.Settings;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Settings
{
    public class UserSettingsVM : BindableBase, IViewModel<UserSettings>
    {
        private readonly IFileSystem fileSystem;

        public UserSettingsVM(IFileSystem fileSystem, UserSettings settings)
        {
            this.fileSystem = fileSystem;

            Model = settings;
            BrowseOutputFolder = new DelegateCommand(OnBrowseOutputFolder);
        }

        public int MaxDuplicateDNAThreshold
        {
            get => Model.MaxDuplicateDNAThreshold;
            set
            {
                if (Model.MaxDuplicateDNAThreshold != value)
                {
                    Model.MaxDuplicateDNAThreshold = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string OutputFolder
        {
            get => Model.OutputFolder;
            set
            {
                if (Model.OutputFolder != value)
                {
                    Model.OutputFolder = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand BrowseOutputFolder { get; }

        public UserSettings Model { get; }

        public bool IsValid()
        {
            return Directory.Exists(OutputFolder);
        }

        private void OnBrowseOutputFolder()
        {
            OutputFolder = fileSystem.SelectFolder();
        }
    }
}
