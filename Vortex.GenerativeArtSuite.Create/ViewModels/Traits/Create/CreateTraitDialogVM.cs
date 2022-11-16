using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Create
{
    public class CreateTraitDialogVM : TraitDialogVM
    {
        public CreateTraitDialogVM(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Title => Strings.CreateTrait;
    }
}
