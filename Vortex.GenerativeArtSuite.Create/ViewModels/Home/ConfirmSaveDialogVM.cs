using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Home
{
    public class ConfirmSaveDialogVM : YesNoDialogVM
    {
        public override string Title => Strings.SaveSession;
    }
}
