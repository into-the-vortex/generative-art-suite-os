using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitVariantVM : NotifyPropertyChanged, IViewModel<TraitVariant>
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
                if(Model.ImagePath != value)
                {
                    Model.ImagePath = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public TraitVariant Model { get; }
    }
}
