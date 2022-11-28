using System;
using System.IO;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.Staging;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Base
{
    public abstract class TraitVM : BindableBase
    {
        private readonly TraitStagingArea traitStagingArea;

        protected TraitVM(IFileSystem fileSystem, TraitStagingArea traitStagingArea, Action raiseCanExecuteChanged)
        {
            this.traitStagingArea = traitStagingArea;

            Icon = new TraitImageVM(
                fileSystem,
                Strings.AddIcon,
                () => traitStagingArea.IconURI.Value,
                val => traitStagingArea.IconURI.Value = val,
                raiseCanExecuteChanged);

            PropertyChanged += (s, e) =>
            {
                raiseCanExecuteChanged();
            };
        }

        public string WeightLabel => $"{Strings.Weight} - {Weight:D3}";

        public string Name
        {
            get => traitStagingArea.Name.Value;
            set
            {
                if (traitStagingArea != null)
                {
                    traitStagingArea.Name.Value = value;
                    RaisePropertyChanged();
                }
            }
        }

        public TraitImageVM Icon { get; }

        public int Weight
        {
            get => traitStagingArea.Weight.Value;
            set
            {
                if (traitStagingArea != null)
                {
                    traitStagingArea.Weight.Value = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(WeightLabel));
                }
            }
        }

        public virtual bool CanConfirm()
        {
            return File.Exists(Icon.URI);
        }
    }
}
