using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Edit
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
