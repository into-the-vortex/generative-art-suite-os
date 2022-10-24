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

        public string WeightLabel => $"{Strings.Weight} - {Weight:D3}";

        public bool IsEditable => Model.Name != Trait.NONENAME;

        public int Weight
        {
            get => Model.Weight;
            set
            {
                if (Model.Weight != value)
                {
                    Model.Weight = value;
                    OnPropertyChanged(nameof(Weight));
                    OnPropertyChanged(nameof(WeightLabel));
                }
            }
        }

        public ObservableCollection<TraitVariantVM> Variants { get; } = new();

        public Trait Model { get; }
    }
}
