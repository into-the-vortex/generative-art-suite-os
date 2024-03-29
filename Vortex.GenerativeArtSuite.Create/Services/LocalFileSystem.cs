﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    internal class LocalFileSystem : IFileSystem
    {
        private const string ROOTPATH = @"%APPDATA%\Vortex Labs\GAS";
        private const string SESSIONSFOLDER = "sessions";
        private const string SESSIONFILE = "session.json";
        private const string USERFILE = "user.json";
        private const string ICONFOLDER = "icons";
        private const string TRAITFOLDER = "traits";
        private const string MASKFOLDER = "masks";

        private readonly string rootPath;
        private readonly string sessionsPath;

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

        public async Task<Session> CreateSession(string remote, Session session)
        {
            await SaveSessionFile(session);
            await SaveUserSettings(session.Name, session.UserSettings);

            session.InitialiseRepository(SessionDirectory(session.Name), remote);

            return session;
        }

        public async Task<Session> CloneSession(string name, string remote, UserSettings userSettings)
        {
            userSettings.GitHandler = GitHandler.Clone(remote, SessionDirectory(name));
            await SaveUserSettings(name, userSettings);

            return await LoadSession(name);
        }

        public async Task<Session> LoadSession(string name)
        {
            var userSettings = await LoadUserSettings(name);
            userSettings.LoadRepository();

            var session = await LoadSessionFile(name);
            session.UserSettings = userSettings;

            return session;
        }

        public async Task SaveSession(string commitMessage, Session session)
        {
            string dir = SessionDirectory(session.Name);

            ManageImages(Path.Combine(dir, ICONFOLDER), session.GetIconURIs());
            ManageImages(Path.Combine(dir, TRAITFOLDER), session.GetTraitURIs());
            ManageImages(Path.Combine(dir, MASKFOLDER), session.GetMaskURIs());

            await SaveSessionFile(session);
            await SaveUserSettings(session.Name, session.UserSettings);

            session.SaveRepository(commitMessage);
        }

        public void DeleteSession(string name)
        {
            var dir = Path.Combine(sessionsPath, name);
            if (Directory.Exists(dir))
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    WorkingDirectory = sessionsPath,
                    Arguments = $"/C rmdir {name} /s /q",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                };

                var process = new Process
                {
                    StartInfo = startInfo,
                };

                process.Start();
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

        private string SessionDirectory(string name)
        {
            var dir = Path.Combine(sessionsPath, name);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }

        private async Task SaveSessionFile(Session session)
        {
            var path = Path.Combine(SessionDirectory(session.Name), SESSIONFILE);
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(session, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            }));
        }

        private async Task<Session> LoadSessionFile(string name)
        {
            var path = Path.Combine(sessionsPath, name, SESSIONFILE);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"{name} does not refer to an existing session", nameof(name));
            }

            return JsonConvert.DeserializeObject<Session>(await File.ReadAllTextAsync(path), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            }) ?? throw new ArgumentException($"{name} could not be loaded", nameof(name));
        }

        private async Task SaveUserSettings(string name, UserSettings userSettings)
        {
            var path = Path.Combine(SessionDirectory(name), USERFILE);
            await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(userSettings, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            }));
        }

        private async Task<UserSettings> LoadUserSettings(string name)
        {
            var path = Path.Combine(SessionDirectory(name), USERFILE);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"{name} does not have any user settings", nameof(name));
            }

            return JsonConvert.DeserializeObject<UserSettings>(await File.ReadAllTextAsync(path), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            }) ?? throw new ArgumentException($"{name} could not load user settings", nameof(name));
        }
    }
}
