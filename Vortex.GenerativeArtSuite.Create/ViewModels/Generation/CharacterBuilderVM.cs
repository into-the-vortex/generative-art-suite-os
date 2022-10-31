using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generation
{
    public class CharacterBuilderVM
    {
        private Session? session;

        public void ConnectSession(Session session)
        {
            if (this.session != session)
            {
                this.session = session;

                // TODO: stuff
            }
        }

    }
}
