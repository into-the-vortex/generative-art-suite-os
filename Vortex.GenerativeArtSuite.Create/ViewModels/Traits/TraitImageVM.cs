using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitImageVM : BindableBase
    {
        private readonly IFileSystem fileSystem;
        private readonly Action<string> set;
        private readonly Func<string> get;

        public TraitImageVM(IFileSystem fileSystem, Func<string> get, Action<string> set, Action raiseCanExecuteChanged)
        {
            this.fileSystem = fileSystem;
            this.get = get;
            this.set = set;

            BrowseImage = new DelegateCommand(OnBrowse);
            ClearImage = new DelegateCommand(OnClear);

            PropertyChanged += (s, e) =>
            {
                raiseCanExecuteChanged();
            };
        }

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
            URI = fileSystem.SelectImageFile();
        }

        private void OnClear()
        {
            URI = string.Empty;
        }
    }
}
