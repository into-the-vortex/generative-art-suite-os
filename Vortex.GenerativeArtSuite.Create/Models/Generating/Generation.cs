﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Create.Models.Settings;

namespace Vortex.GenerativeArtSuite.Create.Models.Generating
{
    public class Generation
    {
        public Generation(int id, string dna, List<GenerationStep> buildOrder)
        {
            Id = id;
            DNA = dna;
            BuildOrder = buildOrder;
        }

        public int Id { get; }

        public string DNA { get; }

        public List<GenerationStep> BuildOrder { get; }

        public void SaveGeneratedImage(IRespectCheckpoint checkpoint, string path)
        {
            checkpoint.RespectCheckpoint();

            using (var bitmap = ImageBuilder.Build(checkpoint, BuildOrder))
            {
                checkpoint.RespectCheckpoint();

                bitmap.Save(Path.Join(path, $"{Id}.png"), ImageFormat.Png);

                checkpoint.RespectCheckpoint();
            }
        }

        public void SaveGeneratedMetadata(IRespectCheckpoint checkpoint, string path, SessionSettings settings)
        {
            var metadata = new ERC721Metadata
            {
                Attributes = BuildOrder.Select(build => build.Trait),
                Compiler = "Vortex Labs",
                // TODO: This time only works in c#, js for example would have a stroke
                Date = DateTime.Now.ToFileTime(),
                Description = settings.DescriptionTemplate,
                Dna = DNA,
                ExternalUrl = null, // TODO: From settings?
                Id = Id,
                Image = Path.Join(settings.BaseURI, $"{Id}.png"),
                Name = $"{settings.NamePrefix} #{Id}",
            };

            checkpoint.RespectCheckpoint();

            File.WriteAllText(Path.Join(path, $"{Id}"), JsonConvert.SerializeObject(metadata, Formatting.Indented));

            checkpoint.RespectCheckpoint();
        }
    }
}