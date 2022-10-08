using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class SessionSettingsVM : NotifyPropertyChanged
    {
        public SessionSettingsVM(SessionSettings settings)
        {
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public ICommand BrowseOutputFolder { get; }

        protected SessionSettings Settings { get; }

        private static string Browse()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.ShowDialog();

            return dialog.SelectedPath;
        }

        private void OnBrowseOutputFolder()
        {
            OutputFolder = Browse();
        }
    }
}
