using System.IO;
using Newtonsoft.Json;

namespace Vortex.GenerativeArtSuite.Create.Models.Settings
{
    public class UserSettings
    {
        private const string IMAGEPATH = "images";
        private const string JSONPATH = "json";

        public UserSettings()
        {
            MaxDuplicateDNAThreshold = 100;
            OutputFolder = string.Empty;
            GitHandler = new();
        }

        [JsonProperty]
        public int MaxDuplicateDNAThreshold { get; set; }

        [JsonProperty]
        public string OutputFolder { get; set; }

        [JsonProperty]
        public GitHandler GitHandler { get; }

        public void InitialiseRepository(string local, string remote)
        {
            GitHandler.InitialiseLocal(local);
            if (!string.IsNullOrWhiteSpace(remote))
            {
                GitHandler.InitialiseRemote(remote);
            }
        }

        public void SaveRepository(string commitMessage)
        {
            GitHandler.Save(commitMessage);
        }

        public void LoadRepository()
        {
            GitHandler.Load();
        }

        public string ImageOutputFolder() => CreateOutputFolder(IMAGEPATH);

        public string JsonOutputFolder() => CreateOutputFolder(JSONPATH);

        private string CreateOutputFolder(string type)
        {
            var folder = Path.Join(OutputFolder, type);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }
    }
}
