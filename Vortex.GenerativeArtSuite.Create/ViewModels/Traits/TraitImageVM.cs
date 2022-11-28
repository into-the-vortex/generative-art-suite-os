using System;
using System.IO;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitImageVM : BindableBase
    {
        private readonly Action? onBrowseSuccess;
        private readonly IFileSystem fileSystem;
        private readonly Action<string> set;
        private readonly Func<string> get;

        public TraitImageVM(
            IFileSystem fileSystem,
            string addPrompt,
            Func<string> get,
            Action<string> set,
            Action raiseCanExecuteChanged,
            Action? onBrowseSuccess = null)
        {
            this.get = get;
            this.set = set;
            this.fileSystem = fileSystem;
            this.onBrowseSuccess = onBrowseSuccess;

            AddPrompt = addPrompt;
            BrowseImage = new DelegateCommand(OnBrowse);
            ClearImage = new DelegateCommand(OnClear);

            PropertyChanged += (s, e) =>
            {
                raiseCanExecuteChanged();
            };
        }

        public string AddPrompt { get; }

        public string URI
        {
            get => get();
            set
            {
                set(value);
                RaisePropertyChanged();
            }
        }

        public ICommand BrowseImage { get; }

        public ICommand ClearImage { get; }

        private void OnBrowse()
        {
            var selected = fileSystem.SelectImageFile();

            if (File.Exists(selected))
            {
                URI = selected;
                onBrowseSuccess?.Invoke();
            }
        }

        private void OnClear()
        {
            URI = string.Empty;
        }
    }
}
