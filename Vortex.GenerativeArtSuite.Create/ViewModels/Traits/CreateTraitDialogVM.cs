using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
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
