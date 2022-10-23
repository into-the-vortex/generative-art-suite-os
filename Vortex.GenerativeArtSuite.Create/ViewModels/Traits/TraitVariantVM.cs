using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class TraitVariantVM : IViewModel<TraitVariant>
    {
        public TraitVariantVM(TraitVariant model)
        {
            Model = model;
        }

        public TraitVariant Model { get; }
    }
}
