using System;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class NewSessionVM : NotifyPropertyChanged
    {
        private readonly Func<string, bool> validateName;
        private SessionSettings settings;
        private string name;

        public NewSessionVM(Func<string, bool> validateName, Action<string, SessionSettings> onCreate)
        {
            this.validateName = validateName;

            name = string.Empty;
            settings = new SessionSettings
            {
                OutputFolder = string.Empty,
                NamePrefix = string.Empty,
                DescriptionTemplate = string.Empty,
                BaseURI = string.Empty,
                CollectionSize = 0,
            };

            var create = new DelegateCommand(() => onCreate(name, settings), CanCreate);
            PropertyChanged += (s, e) =>
            {
                create.RaiseCanExecuteChanged();
            };

            Create = create;
            BrowseOutputFolder = new DelegateCommand(OnBrowseOutputFolder);
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OutputFolder
        {
            get => settings.OutputFolder;
            set
            {
                if (settings.OutputFolder != value)
                {
                    settings.OutputFolder = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NamePrefix
        {
            get => settings.NamePrefix;
            set
            {
                if (settings.NamePrefix != value)
                {
                    settings.NamePrefix = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DescriptionTemplate
        {
            get => settings.DescriptionTemplate;
            set
            {
                if (settings.DescriptionTemplate != value)
                {
                    settings.DescriptionTemplate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BaseURI
        {
            get => settings.BaseURI;
            set
            {
                if (settings.BaseURI != value)
                {
                    settings.BaseURI = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CollectionSize
        {
            get => settings.CollectionSize;
            set
            {
                if (settings.CollectionSize != value)
                {
                    settings.CollectionSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand Create { get; }

        public ICommand BrowseOutputFolder { get; }

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

        private bool CanCreate()
        {
            return validateName(name) &&
                !string.IsNullOrWhiteSpace(OutputFolder) &&
                !string.IsNullOrWhiteSpace(NamePrefix) &&
                !string.IsNullOrWhiteSpace(DescriptionTemplate) &&
                !string.IsNullOrWhiteSpace(BaseURI) &&
                CollectionSize > 0;
        }
    }
}
