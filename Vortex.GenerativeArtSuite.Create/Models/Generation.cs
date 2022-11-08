using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Common.Models;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class Generation
    {
        private const string IMAGEPATH = "images";
        private const string JSONPATH = "json";

        public Generation(string dna, List<GenerationStep> buildOrder, List<string> chosenPaths)
        {
            DNA = dna;
            BuildOrder = buildOrder;
            ChosenPaths = chosenPaths;
        }

        public string DNA { get; }

        public List<GenerationStep> BuildOrder { get; }

        public List<string> ChosenPaths { get; }

        public void GenerateImage(IGenerationProcess process, int id, SessionSettings settings)
        {
            process.RespectCheckpoint();

            var folder = Path.Join(settings.OutputFolder, IMAGEPATH);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            process.RespectCheckpoint();

            var path = Path.Join(folder, $"{id}.png");

            using (var bitmap = ImageBuilder.Build(process, BuildOrder))
            {
                process.RespectCheckpoint();
                bitmap.Save(path, ImageFormat.Png);
                process.RespectCheckpoint();
            }
        }

        public void GenerateJson(IGenerationProcess process, int id, SessionSettings settings)
        {
            process.RespectCheckpoint();

            var folder = Path.Join(settings.OutputFolder, JSONPATH);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            process.RespectCheckpoint();

            var path = Path.Join(folder, $"{id}.json");
            var metadata = new ERC721Metadata
            {
                Attributes = BuildOrder.Select(build => build.Trait),
                // TODO: This time only works in c#, js for example would have a stroke
                Date = DateTime.Now.ToFileTime(),
                Description = settings.DescriptionTemplate,
                Dna = DNA,
                ExternalUrl = "TODO",
                Id = id,
                Image = Path.Join(settings.BaseURI, $"{id}.png"),
                Name = $"{settings.NamePrefix} #{id}",
                Paths = ChosenPaths,
            };

            process.RespectCheckpoint();

            File.WriteAllText(path, JsonConvert.SerializeObject(metadata, Formatting.Indented));
        }
    }
}
