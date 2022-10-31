using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generation
{
    public class GenerateVM : SessionAwareVM
    {
        public CharacterBuilderVM CharacterBuilderVM { get; } = new();

        public GenerationVM GenerationVM { get; } = new();

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            CharacterBuilderVM.ConnectSession(Session());
            GenerationVM.ConnectSession(Session());
        }
    }
}
