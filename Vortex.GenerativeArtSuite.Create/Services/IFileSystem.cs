using System.Collections.Generic;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface IFileSystem
    {
        IEnumerable<RecentSession> RecentSessions();

        Session CreateSession(string name, string remote, SessionSettings sessionSettings);

        Session LoadSession(string name);

        void SaveSession(Session session);

        string SelectFolder();

        string SelectImageFile();

        string SaveFile(string filter, string defaultExt);
    }
}
