using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    internal class SessionManager : ISessionManager
    {
        private readonly Dictionary<Type, string> dirtyChecker = new();
        private readonly IFileSystem fileSystem;

        private Session? session;

        public SessionManager(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool RequiresReset(Type viewer)
        {
            if (session is null)
            {
                return false;
            }

            var current = JsonConvert.SerializeObject(session);
            if (!dirtyChecker.ContainsKey(viewer) || dirtyChecker[viewer] != current)
            {
                dirtyChecker[viewer] = current;
                return true;
            }

            return false;
        }

        public void CreateNewSession(string name, string remote, SessionSettings sessionSettings)
        {
            session = fileSystem.CreateSession(name, remote, sessionSettings);
            OnNewSessionContext();
        }

        public void OpenExistingSession(string name)
        {
            session = fileSystem.LoadSession(name);
            OnNewSessionContext();
        }

        public bool CanSaveSession()
        {
            if (dirtyChecker[typeof(SessionManager)] is null || session is null)
            {
                return false;
            }

            return dirtyChecker[typeof(SessionManager)] != JsonConvert.SerializeObject(session);
        }

        public void SaveSession()
        {
            fileSystem.SaveSession(Session());
            dirtyChecker[typeof(SessionManager)] = JsonConvert.SerializeObject(session);
        }

        public Session Session()
        {
            if (session is null)
            {
                throw new InvalidOperationException("Session is not set");
            }

            return session;
        }

        private void OnNewSessionContext()
        {
            dirtyChecker.Clear();
            dirtyChecker[typeof(SessionManager)] = JsonConvert.SerializeObject(session);
        }
    }
}
