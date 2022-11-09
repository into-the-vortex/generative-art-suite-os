using System;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    internal class SessionManager : ISessionManager
    {
        private readonly IFileSystem fileSystem;

        private Session? session;

        public SessionManager(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void CreateNewSession(string name, SessionSettings sessionSettings)
        {
            session = new Session(name, sessionSettings);
            fileSystem.SaveSession(session);
        }

        public void OpenExistingSession(string name)
        {
            session = fileSystem.LoadSession(name);
        }

        public void SaveSession()
        {
            fileSystem.SaveSession(Session());
        }

        public Session Session()
        {
            if (session is null)
            {
                throw new InvalidOperationException("Session is not set");
            }

            return session;
        }
    }
}
