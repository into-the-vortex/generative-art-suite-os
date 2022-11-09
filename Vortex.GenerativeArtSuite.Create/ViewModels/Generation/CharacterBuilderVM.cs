using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generation
{
    public class CharacterBuilderVM : BindableBase
    {
        private readonly ISessionProvider sessionProvider;

        public CharacterBuilderVM(ISessionProvider sessionProvider)
        {
            this.sessionProvider = sessionProvider;
        }

        public void Reset()
        {

        }
    }
}
