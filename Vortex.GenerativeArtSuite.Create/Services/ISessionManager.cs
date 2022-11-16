using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface ISessionManager : ISessionProvider
    {
        void CreateNewSession(string name, SessionSettings sessionSettings);

        void OpenExistingSession(string name);
    }
}
