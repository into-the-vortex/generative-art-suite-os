using System.Collections.Generic;
using System.Threading.Tasks;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface IFileSystem
    {
        IEnumerable<RecentSession> RecentSessions();

        Task<Session> CreateSession(string remote, Session session);

        Task<Session> CloneSession(string name, string remote, UserSettings userSettings);

        Task<Session> LoadSession(string name);

        Task SaveSession(string commitMessage, Session session);

        void DeleteSession(string name);

        string SelectFolder();

        string SelectImageFile();

        string SaveFile(string filter, string defaultExt);
    }
}
