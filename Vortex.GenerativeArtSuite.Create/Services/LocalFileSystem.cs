using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    internal class LocalFileSystem : IFileSystem
    {
        private const string ROOTPATH = @"%APPDATA%\Vortex Labs\GAS";
        private const string SESSIONFILE = "session.json";
        private const string SESSIONSFOLDER = "sessions";
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

                ManageImages(Path.Combine(dir, ICONFOLDER), session.GetIconURIs());
                ManageImages(Path.Combine(dir, TRAITFOLDER), session.GetTraitURIs());
                ManageImages(Path.Combine(dir, MASKFOLDER), session.GetMaskURIs());

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
    }
}
