using System.Threading.Tasks;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface ISessionManager : ISessionProvider
    {
        Task CreateNewSession(string remote, Session session);

        Task CloneNewSession(string name, string remote, UserSettings userSettings);

        Task OpenExistingSession(string name);
    }
}
