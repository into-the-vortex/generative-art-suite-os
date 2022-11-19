using Vortex.GenerativeArtSuite.Create.Models.Sessions;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface ISessionManager : ISessionProvider
    {
        void CreateNewSession(string remote, Session session);

        void OpenExistingSession(string name);
    }
}
