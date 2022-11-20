using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface ISessionManager : ISessionProvider
    {
        void CreateNewSession(string remote, Session session);

        void CloneNewSession(string name, string remote, UserSettings userSettings);

        void OpenExistingSession(string name);
    }
}
