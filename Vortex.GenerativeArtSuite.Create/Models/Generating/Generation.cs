using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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
        private readonly List<Layer> layers;

        public Generation(int id, string dna, List<Layer> layers, List<GenerationStep> buildOrder)
        {
            Id = id;
            DNA = dna;
            BuildOrder = buildOrder;

            this.layers = layers;
        }

        public int Id { get; }

        public string DNA { get; }

        public List<GenerationStep> BuildOrder { get; }

        public Bitmap GenerateImage(IGenerationProcess process)
        {
            return ImageBuilder.Build(process, BuildOrder);
        }

        public void SaveGeneratedImage(IGenerationProcess process, string path)
        {
            process.RespectCheckpoint();

            using (var bitmap = GenerateImage(process))
            {
                process.RespectCheckpoint();

                bitmap.Save(Path.Join(path, $"{Id}.png"), ImageFormat.Png);

                process.RespectCheckpoint();
            }
        }

        public ERC721Metadata GenerateMetadata(GenerationSettings settings)
        {
            return new ERC721Metadata
            {
                Attributes = BuildOrder.Select(build => build.Trait),
                Compiler = "Vortex Labs",
                Date = DateTime.Now,
                Description = settings.DescriptionTemplate,
                Dna = DNA,
                ExternalUrl = string.Format(CultureInfo.InvariantCulture, settings.ExternalUrl, Id),
                Id = Id,
                Image = Path.Join(settings.BaseURI, $"{Id}.png"),
                Name = $"{settings.NamePrefix} #{Id}",
            };
        }

        public void SaveGeneratedMetadata(IGenerationProcess checkpoint, string path, GenerationSettings settings)
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

        public Generation ApplyRules()
        {
            var notRequired = new List<GenerationStep>();
            var item = BuildOrder[7];
            var armLayer = layers.First(l => l.Name == "Arm");

            if (item.Trait.TraitName == "None")
            {
                BuildOrder[1] = armLayer.Traits[0].CreateGenerationStep(armLayer, notRequired);
            }
            else
            {
                var skin = BuildOrder[2];
                switch (skin.Trait.TraitName)
                {
                    case "None":
                        BuildOrder[1] = armLayer.Traits[0].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Blue Two-Toned Curly":
                        BuildOrder[1] = armLayer.Traits[1].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Brown Quetzalcoatl":
                        BuildOrder[1] = armLayer.Traits[2].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Brown":
                        BuildOrder[1] = armLayer.Traits[3].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Galaxy":
                        BuildOrder[1] = armLayer.Traits[4].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Magma":
                        BuildOrder[1] = armLayer.Traits[5].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Green":
                        BuildOrder[1] = armLayer.Traits[6].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Pink":
                        BuildOrder[1] = armLayer.Traits[7].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Tweeter":
                        BuildOrder[1] = armLayer.Traits[8].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Cream":
                        BuildOrder[1] = armLayer.Traits[9].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Green Curly":
                        BuildOrder[1] = armLayer.Traits[10].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Quetzalcoatl":
                        BuildOrder[1] = armLayer.Traits[11].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Red Curly":
                        BuildOrder[1] = armLayer.Traits[12].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Rock":
                        BuildOrder[1] = armLayer.Traits[13].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Tan Shades":
                        BuildOrder[1] = armLayer.Traits[14].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Water":
                        BuildOrder[1] = armLayer.Traits[15].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "White Quetzalcoatl":
                        BuildOrder[1] = armLayer.Traits[16].CreateGenerationStep(armLayer, notRequired);
                        break;
                    case "Yellow":
                        BuildOrder[1] = armLayer.Traits[17].CreateGenerationStep(armLayer, notRequired);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            return this;
        }

        public bool IsValidConfig()
        {
            var troublesomeEyes = new string[]
            {
                "3d Glasses",
                "Wooden Glasses",
                "Red Glasses",
            };
            var troublesomeHead = new string[]
            {
                "Green Croc Headdress",
                "Red Croc Headdress",
                "Blue Eagle Headdress",
                "Red Eagle Headdress",
                "Golden Headdress",
                "Green Headdress",
                "Blue Headdress",
                "Leopard Headdress",
                "Golden Leopard Headdress",
            };

            var eyes = BuildOrder[3];
            var head = BuildOrder[5];

            return !(troublesomeEyes.Contains(eyes.Trait.TraitName) && troublesomeHead.Contains(head.Trait.TraitName));
        }
    }
}
