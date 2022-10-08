using System.Collections.Generic;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    internal struct SubLayer
    {
        public string DisplayName { get; set; }

        public Gene? Gene { get; set; }

        public IEnumerable<Gene> RequiredGenes { get; set; }
    }
}
