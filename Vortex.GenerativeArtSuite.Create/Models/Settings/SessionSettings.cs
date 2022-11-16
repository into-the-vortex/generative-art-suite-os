using System.IO;

namespace Vortex.GenerativeArtSuite.Create.Models.Settings
{
    public class SessionSettings
    {
        private const string IMAGEPATH = "images";
        private const string JSONPATH = "json";
        private const string BUILDERPATH = "builder";

        public SessionSettings()
        {
            OutputFolder = string.Empty;
            NamePrefix = string.Empty;
            DescriptionTemplate = string.Empty;
            BaseURI = string.Empty;
            CollectionSize = 10000;
        }

        public string OutputFolder { get; set; }

        public string NamePrefix { get; set; }

        public string DescriptionTemplate { get; set; }

        public string BaseURI { get; set; }

        public int CollectionSize { get; set; }

        public string ImageOutputFolder() => CreateOutputFolder(IMAGEPATH);

        public string JsonOutputFolder() => CreateOutputFolder(JSONPATH);

        public string BuilderOutputFolder() => CreateOutputFolder(BUILDERPATH);

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
