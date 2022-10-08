using System.Collections.Generic;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    internal struct Layer
    {
        public string Name { get; set; }

        public IEnumerable<SubLayer> Branches { get; set; }

        public bool Optional { get; set; }
    }
}
