using System.Collections.ObjectModel;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitVM : NotifyPropertyChanged, IViewModel<Trait>
    {
        public TraitVM(Trait model)
        {
            Model = model;
            Variants.ConnectModelCollection(model.Variants, (v) => new TraitVariantVM(v));
        }

        public string Name => Model.Name;

        public string IconURI => Model.IconURI;

        public double Weight
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

        public ObservableCollection<TraitVariantVM> Variants { get; } = new();

        public Trait Model { get; }
    }
}
