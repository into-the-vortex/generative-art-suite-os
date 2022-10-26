using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitVariantVM : BindableBase, IViewModel<TraitVariant>
    {
        public TraitVariantVM(TraitVariant model)
        {
            Model = model;
        }

        public string DisplayName => Model.DisplayName;

        public string ImagePath
        {
            get => Model.ImagePath;
            set
            {
                if (Model.ImagePath != value)
                {
                    Model.ImagePath = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string MaskPath
        {
            get => Model.MaskPath;
            set
            {
                if (Model.MaskPath != value)
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

        public TraitVariant Model { get; }
    }
}
