using System.Collections.Generic;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface IFileSystem
    {
        IEnumerable<RecentSession> RecentSessions();

        Session LoadSession(string name);

        void SaveSession(Session session);

        string SelectFolder();

        string SelectImageFile();
    }
}
