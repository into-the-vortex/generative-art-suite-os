using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits
{
    public class EditTraitDialogVM : TraitDialogVM
    {
        public EditTraitDialogVM(IFileSystem fileSystem)
            : base(fileSystem)
        {
        }

        public override string Title => Strings.EditTrait;
    }
}
