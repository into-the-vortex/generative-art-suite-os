using System.IO;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitVariantVM : BindableBase, IViewModel<TraitVariant>
    {
        private readonly IFileSystem fileSystem;

        public TraitVariantVM(IFileSystem fileSystem, TraitVariant model)
        {
            this.fileSystem = fileSystem;

            Model = model;
            BrowseImage = new DelegateCommand(OnBrowseImage);
            ClearImage = new DelegateCommand(OnClearImage);
            BrowseMask = new DelegateCommand(OnBrowseMask);
            ClearMask = new DelegateCommand(OnClearMask);
        }

        public string DisplayName => Model.DisplayName;

        public string? ImagePath
        {
            get => Model.ImagePath;
            set
            {
                if (Model.ImagePath != value && (value is null || File.Exists(value)))
                {
                    Model.ImagePath = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string? MaskPath
        {
            get => Model.MaskPath;
            set
            {
                if (Model.MaskPath != value && (value is null || File.Exists(value)))
                {
                    Model.MaskPath = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int Weight
        {
            get => Model.Weight;
            set
            {
                if (Model.Weight != value)
                {
                    Model.Weight = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(WeightLabel));
                }
            }
        }

        public string WeightLabel => $"{Strings.Weight} - {Weight:D3}";

        public ICommand BrowseImage { get; }

        public ICommand ClearImage { get; }

        public ICommand BrowseMask { get; }

        public ICommand ClearMask { get; }

        public TraitVariant Model { get; }

        private void OnBrowseImage()
        {
            ImagePath = fileSystem.SelectImageFile();
        }

        private void OnClearImage()
        {
            ImagePath = null;
        }

        private void OnBrowseMask()
        {
            MaskPath = fileSystem.SelectImageFile();
        }

        private void OnClearMask()
        {
            MaskPath = null;
        }
    }
}
