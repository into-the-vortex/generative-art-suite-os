using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generation
{
    public class GenerateVM : SessionAwareVM
    {
        public GenerateVM(ISessionProvider sessionProvider, INavigationLock navigationLock)
            : base(sessionProvider)
        {
            CharacterBuilderVM = new(sessionProvider);
            GenerationVM = new(sessionProvider, navigationLock);
        }

        public CharacterBuilderVM CharacterBuilderVM { get; }

        public GenerationVM GenerationVM { get; }

        protected override void ResetOnSessionChanged()
        {
            CharacterBuilderVM.Reset();
            GenerationVM.Reset();
        }
    }
}
