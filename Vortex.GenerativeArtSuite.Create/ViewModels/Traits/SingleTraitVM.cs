using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class SingleTraitVM : IViewModel<Trait>
    {
        public SingleTraitVM(Trait model)
        {
            Model = model;
        }

        public string DisplayName { get; }

        public bool HasMask { get; }

        public Trait Model { get; }
    }
}
