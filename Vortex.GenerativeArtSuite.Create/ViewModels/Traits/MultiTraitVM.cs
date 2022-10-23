using System.Collections.Generic;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class MultiTraitVM
    {
        private IEnumerable<Trait> enumerable;
        private List<string> paths;

        public MultiTraitVM(IEnumerable<Trait> enumerable, List<string> paths)
        {
            this.enumerable = enumerable;
            this.paths = paths;
        }
    }
}
