using System.Collections.ObjectModel;
using Vortex.GenerativeArtSuite.Common.Extensions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitsVM : IViewModel<Layer>
    {
        public TraitsVM(Layer layer)
        {
            Model = layer;
            TraitVms.ConnectModelCollection(layer.Traits, (t) => new TraitVM(t));
        }

        public ObservableCollection<TraitVM> TraitVms { get; } = new();

        public Layer Model { get; }
    }
}
