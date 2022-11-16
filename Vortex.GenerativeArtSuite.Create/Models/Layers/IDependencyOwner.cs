using System.Collections.Generic;

namespace Vortex.GenerativeArtSuite.Create.Models.Layers
{
    public interface IDependencyOwner
    {
        List<Dependency> Dependencies { get; }
    }
}
