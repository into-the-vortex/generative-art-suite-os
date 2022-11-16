﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using DynamicData;
using Newtonsoft.Json;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Create.Models.Layers;
using Vortex.GenerativeArtSuite.Create.Models.Settings;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

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

        public Bitmap GenerateImage(IRespectCheckpoint checkpoint)
        {
            return ImageBuilder.Build(checkpoint, BuildOrder);
        }

        public void SaveGeneratedImage(IRespectCheckpoint checkpoint, string path)
        {
            checkpoint.RespectCheckpoint();

            using (var bitmap = GenerateImage(checkpoint))
            {
                checkpoint.RespectCheckpoint();

                bitmap.Save(Path.Join(path, $"{Id}.png"), ImageFormat.Png);

                checkpoint.RespectCheckpoint();
            }
        }

        public ERC721Metadata GenerateMetadata(SessionSettings settings)
        {
            return new ERC721Metadata
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
        }

        public void SaveGeneratedMetadata(IRespectCheckpoint checkpoint, string path, SessionSettings settings)
        {
            var metadata = GenerateMetadata(settings);

            checkpoint.RespectCheckpoint();

            File.WriteAllText(Path.Join(path, $"{Id}"), JsonConvert.SerializeObject(metadata, Formatting.Indented));
            checkpoint.RespectCheckpoint();
        }

        public bool ContainsTrait(string layer, string trait) =>
            BuildOrder.Any(b => b.Trait.LayerName == layer) &&
            BuildOrder.First(b => b.Trait.LayerName == layer).Trait.TraitName == trait;

        public Generation SwapTrait(Layer layer, Trait trait)
        {
            var oldStep = BuildOrder.First(b => b.Trait.LayerName == layer.Name);

            var index = BuildOrder.IndexOf(oldStep);

            var newStep = trait.CreateGenerationStep(layer, BuildOrder.Take(index).ToList());

            BuildOrder.Replace(
                oldStep,
                newStep);

            return this;
        }
    }
}
