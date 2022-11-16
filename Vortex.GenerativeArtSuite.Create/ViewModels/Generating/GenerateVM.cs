using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generating
{
    public class GenerateVM : SessionAwareVM
    {
        private readonly ISessionProvider sessionProvider;

        public GenerateVM(IDialogService dialogService, ISessionProvider sessionProvider, INavigationLock navigationLock)
            : base(sessionProvider, dialogService)
        {
            this.sessionProvider = sessionProvider;

            CharacterBuilderVM = new(sessionProvider);
            GenerationVM = new(sessionProvider, navigationLock);
        }

        public CharacterBuilderVM CharacterBuilderVM { get; }

        public GenerationVM GenerationVM { get; }

        protected override void ResetOnSessionChanged()
        {
            ImageBuilder.BuildCache(sessionProvider.Session());

            CharacterBuilderVM.Reset();
            GenerationVM.Reset();
        }
    }
}
