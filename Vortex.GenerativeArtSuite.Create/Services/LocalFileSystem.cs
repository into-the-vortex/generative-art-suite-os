using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

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
                throw new ArgumentException($"{name} does not refer to an existing session", nameof(name));
            }

            return JsonConvert.DeserializeObject<Session>(File.ReadAllText(path), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            }) ?? throw new ArgumentException($"{name} could not be loaded", nameof(name));
        }

        public void SaveSession(Session session)
        {
            try
            {
                var dir = Path.Combine(sessionsPath, session.Name);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var path = Path.Combine(dir, SESSIONFILE);
                File.WriteAllText(path, JsonConvert.SerializeObject(session, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                }));
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
    }
}
