using System.Collections.Generic;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class Layer
    {
        public Layer(string name, bool optional, bool includeInDNA, bool affectedByLayerMask, List<PathSelector> paths)
        {
            Name = name;
            Optional = optional;
            IncludeInDNA = includeInDNA;
            AffectedByLayerMask = affectedByLayerMask;
            Paths = paths;
        }

        public string Name { get; set; }

        public bool Optional { get; set; }

        public bool IncludeInDNA { get; set; }

        public bool AffectedByLayerMask { get; set; }

        public List<PathSelector> Paths { get; }
    }
}
