using System;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.Staging;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class PathingTraitVM : TraitVM
    {
        public PathingTraitVM(IFileSystem fileSystem, PathingTraitStagingArea traitStagingArea, Action raiseCanExecuteChanged)
            : base(fileSystem, traitStagingArea, raiseCanExecuteChanged)
        {
        }
    }
}
