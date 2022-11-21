using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public event Action<bool>? OnBusyChanged;

        public async Task CreateNewSession(string remote, Session session)
        {
            using (var busy = new Busy(val => OnBusyChanged?.Invoke(val)))
            {
                this.session = await fileSystem.CreateSession(remote, session);
                OnNewSessionContext();
            }
        }

        public async Task CloneNewSession(string name, string remote, UserSettings userSettings)
        {
            using (var busy = new Busy(val => OnBusyChanged?.Invoke(val)))
            {
                session = await fileSystem.CloneSession(name, remote, userSettings);
                OnNewSessionContext();
            }
        }

        public async Task OpenExistingSession(string name)
        {
            using (var busy = new Busy(val => OnBusyChanged?.Invoke(val)))
            {
                session = await fileSystem.LoadSession(name);
                OnNewSessionContext();
            }
        }

        public async Task SaveSession(string commitMessage)
        {
            using (var busy = new Busy(val => OnBusyChanged?.Invoke(val)))
            {
                await fileSystem.SaveSession(commitMessage, Session());
                dirtyChecker[typeof(SessionManager)] = JsonConvert.SerializeObject(session);
            }
        }

        public bool CanSaveSession()
        {
            if (dirtyChecker[typeof(SessionManager)] is null || session is null)
            {
                return false;
            }

            return dirtyChecker[typeof(SessionManager)] != JsonConvert.SerializeObject(session);
        }

        public Session Session()
        {
            if (session is null)
            {
                throw new InvalidOperationException("Session is not set");
            }

            return session;
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

        private void OnNewSessionContext()
        {
            dirtyChecker.Clear();
            dirtyChecker[typeof(SessionManager)] = JsonConvert.SerializeObject(session);
        }

        private class Busy : IDisposable
        {
            private readonly Action<bool> fireEvent;

            public Busy(Action<bool> fireEvent)
            {
                this.fireEvent = fireEvent;

                fireEvent(true);
            }

            public void Dispose()
            {
                fireEvent(false);
            }
        }
    }
}
