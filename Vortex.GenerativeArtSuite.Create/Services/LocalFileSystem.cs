using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    internal class LocalFileSystem : IFileSystem
    {
        private const string SESSIONFILE = "session.json";

        private readonly string rootPath;
        private readonly string sessionsPath;

        public LocalFileSystem()
        {
            rootPath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\Vortex Labs\GAS");
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            sessionsPath = Path.Combine(rootPath, "sessions");
            if (!Directory.Exists(sessionsPath))
            {
                Directory.CreateDirectory(sessionsPath);
            }
        }

        public IEnumerable<RecentSession> RecentSessions()
        {
            var directories = Directory.GetDirectories(sessionsPath);
            foreach (var directory in directories)
            {
                var info = new DirectoryInfo(directory);
                yield return new RecentSession()
                {
                    Name = info.Name,
                    Created = info.CreationTime,
                    Modified = info.LastWriteTime,
                };
            }
        }

        public Session LoadSession(string name)
        {
            var path = Path.Combine(sessionsPath, name, SESSIONFILE);

            if (!File.Exists(path))
            {
                throw new ArgumentException("Session name does not exist", nameof(name));
            }

            return JsonConvert.DeserializeObject<Session>(File.ReadAllText(path)) ?? throw new ArgumentException("Session could not be loaded", nameof(name));
        }

        public void SaveSession(Session session)
        {
            var dir = Path.Combine(sessionsPath, session.Name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var path = Path.Combine(dir, SESSIONFILE);
            File.WriteAllText(path, JsonConvert.SerializeObject(session));
        }
    }
}
