using System.Collections.Generic;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class Layer
    {
        private List<SubLayer> subLayers = new();

        public Layer(int index, string name, bool optional, bool includeInDNA)
        {
            Index = index;
            Name = name;
            Optional = optional;
            IncludeInDNA = includeInDNA;
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public bool Optional { get; set; }

        public bool IncludeInDNA { get; set; }

        public IEnumerable<SubLayer> Branches => subLayers;

        public void AddBranch(SubLayer branch)
        {
            subLayers.Add(branch);
        }

        public void DeleteBranch(SubLayer branch)
        {
            subLayers.Remove(branch);
        }
    }
}
