using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    internal class LocalFileSystem : IFileSystem
    {
        private const string ROOTPATH = @"%APPDATA%\Vortex Labs\GAS";
        private const string GITHANDLERFOLDER = "githandles";
        private const string SESSIONSFOLDER = "sessions";
        private const string SESSIONFILE = "session.json";
        private const string ICONFOLDER = "icons";
        private const string TRAITFOLDER = "traits";
        private const string MASKFOLDER = "masks";

        private readonly string rootPath;
        private readonly string sessionsPath;
        private readonly string gitHandlerPath;

        public LocalFileSystem()
        {
            rootPath = Environment.ExpandEnvironmentVariables(ROOTPATH);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            sessionsPath = Path.Combine(rootPath, SESSIONSFOLDER);
            if (!Directory.Exists(sessionsPath))
            {
                Directory.CreateDirectory(sessionsPath);
            }

            gitHandlerPath = Path.Combine(rootPath, GITHANDLERFOLDER);
            if (!Directory.Exists(gitHandlerPath))
            {
                Directory.CreateDirectory(gitHandlerPath);
            }
        }

        public IEnumerable<RecentSession> RecentSessions()
        {
            // TODO: clean old githandlers
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

        public Session CreateSession(string name, string remote, SessionSettings sessionSettings)
        {
            var session = new Session(name, sessionSettings);

            string dir = SessionDirectory(session);
            SaveSessionFile(session, dir);

            session.GitHandler.InitialiseLocal(dir);
            if (!string.IsNullOrWhiteSpace(remote))
            {
                session.GitHandler.InitialiseRemote(remote);
            }

            SaveGitHandler(name, session.GitHandler);

            return session;
        }

        public Session LoadSession(string name)
        {
            var path = Path.Combine(sessionsPath, name, SESSIONFILE);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"{name} does not refer to an existing session", nameof(name));
            }

            var session = JsonConvert.DeserializeObject<Session>(File.ReadAllText(path), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            }) ?? throw new ArgumentException($"{name} could not be loaded", nameof(name));

            session.GitHandler = LoadGitHandler(name);
            session.GitHandler.Load();

            return session;
        }

        public void SaveSession(Session session)
        {
            try
            {
                string dir = SessionDirectory(session);

                ManageImages(Path.Combine(dir, ICONFOLDER), session.GetIconURIs());
                ManageImages(Path.Combine(dir, TRAITFOLDER), session.GetTraitURIs());
                ManageImages(Path.Combine(dir, MASKFOLDER), session.GetMaskURIs());

                SaveSessionFile(session, dir);

                session.GitHandler.Save();
                SaveGitHandler(session.Name, session.GitHandler);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unexpected error saving the session: " + e);
            }
        }

        public string SelectFolder()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.ShowDialog();

            return dialog.SelectedPath;
        }

        public string SelectImageFile()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog
            {
                Multiselect = false,
                Filter = "Image Files|*.BMP;*.JPEG;*.PNG;",
            };
            dialog.ShowDialog();

            return dialog.FileName;
        }

        public string SaveFile(string filter, string defaultExt)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaSaveFileDialog
            {
                Filter = filter,
                AddExtension = true,
                DefaultExt = defaultExt,
            };
            dialog.ShowDialog();

            return dialog.FileName;
        }

        private static void SaveSessionFile(Session session, string dir)
        {
            var path = Path.Combine(dir, SESSIONFILE);
            File.WriteAllText(path, JsonConvert.SerializeObject(session, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            }));
        }

        private static void CleanUnreferenced(string dir, List<Reference<string>> images)
        {
            var files = Directory.GetFiles(dir);

            foreach (var file in files)
            {
                if (!images.Any(img => img.Value == file))
                {
                    File.Delete(file);
                }
            }
        }

        private void ManageImages(string dir, List<Reference<string>> images)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            CleanUnreferenced(dir, images);
            SaveAsRelative(dir, images);
        }

        private void SaveAsRelative(string dir, List<Reference<string>> images)
        {
            var files = Directory.GetFiles(dir);

            foreach (var image in images)
            {
                if (!files.Contains(image.Value))
                {
                    var name = $"{Guid.NewGuid()}{new FileInfo(image.Value).Extension}";
                    var newPath = Path.Join(dir, name);
                    File.Copy(image.Value, newPath);
                    image.Value = newPath.Replace(rootPath, ROOTPATH);
                }
            }
        }

        private string SessionDirectory(Session session)
        {
            var dir = Path.Combine(sessionsPath, session.Name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }

        private void SaveGitHandler(string name, GitHandler handler)
        {
            var path = Path.Combine(gitHandlerPath, $"{name}.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(handler, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            }));
        }

        private GitHandler LoadGitHandler(string name)
        {
            var path = Path.Combine(gitHandlerPath, $"{name}.json");

            if (!File.Exists(path))
            {
                throw new ArgumentException($"{name} does not refer to an existing git handler", nameof(name));
            }

            return JsonConvert.DeserializeObject<GitHandler>(File.ReadAllText(path), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            }) ?? throw new ArgumentException($"{name} could not be loaded", nameof(name));
        }
    }
}
