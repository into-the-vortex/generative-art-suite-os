using System;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DynamicData;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Models.Generating;
using Vortex.GenerativeArtSuite.Create.Models.Layers;
using Vortex.GenerativeArtSuite.Create.Models.Traits;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generating
{
    public class CharacterBuilderVM : BindableBase
    {
        private readonly ISessionProvider sessionProvider;
        private readonly IFileSystem fileSystem;
        private ImageGenerationProcess? process;
        private Generation? generation;
        private string? selectedLayer;
        private byte[] image = Array.Empty<byte>();
        private string json = string.Empty;

        public CharacterBuilderVM(ISessionProvider sessionProvider, IFileSystem fileSystem)
        {
            this.sessionProvider = sessionProvider;
            this.fileSystem = fileSystem;
            Save = new DelegateCommand(OnSave, CanSave)
                .ObservesProperty(() => Json)
                .ObservesProperty(() => Image);
            Randomise = new DelegateCommand(OnRandomise);
        }

        public string Json
        {
            get => json;
            set => SetProperty(ref json, value);
        }

        public byte[] Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }

        public string? SelectedLayer
        {
            get => selectedLayer;
            set => SetProperty(ref selectedLayer, value, UpdateSelection);
        }

        public ICommand Randomise { get; }

        public ICommand Save { get; }

        public ObservableCollection<string> Layers { get; } = new();

        public ObservableCollection<CharacterTraitVM> Traits { get; } = new();

        public void Reset()
        {
            Layers.Clear();
            Layers.AddRange(sessionProvider.Session().Layers.Select(l => l.Name));

            SelectedLayer = Layers.FirstOrDefault();

            OnRandomise();
        }

        private void Redraw()
        {
            Image = Array.Empty<byte>();
            Json = string.Empty;

            StartGeneration();
        }

        private void UpdateSelection()
        {
            Traits.Clear();

            var layer = GetSelectedLayer();
            if (layer is not null)
            {
                Traits.AddRange(layer
                    .Traits
                    .Select(t => new CharacterTraitVM(t, generation?.ContainsTrait(layer.Name, t.Name) != false, OnTraitClicked)));
            }
        }

        private void OnTraitClicked(Trait trait)
        {
            var layer = GetSelectedLayer();

            if (layer is not null && generation is not null)
            {
                generation = sessionProvider
                    .Session()
                    .CreateGeneration(0, generation.SwapTrait(layer, trait).BuildOrder);

                Redraw();
            }
        }

        private void StartGeneration()
        {
            if (generation is null)
            {
                throw new InvalidOperationException();
            }

            if (process is not null)
            {
                process.ProcessComplete -= OnSuccess;
                process.Cancel();
                process = null;
            }

            process = new ImageGenerationProcess();
            process.ProcessComplete += OnSuccess;

            Task.Run(() =>
            {
                var settings = sessionProvider.Session().Settings;
                var output = settings.BuilderOutputFolder();

                Json = JsonConvert.SerializeObject(generation.GenerateMetadata(settings), Formatting.Indented);

                using (var image = generation.GenerateImage(process))
                {
                    using (var stream = new MemoryStream())
                    {
                        image.Save(stream, ImageFormat.Png);
                        Image = stream.ToArray();
                    }
                }

                process.Complete();
            });
        }

        private void OnRandomise()
        {
            generation = sessionProvider.Session().CreateRandomGeneration(0);
            Redraw();
        }

        private void OnSave()
        {
            var imageSave = fileSystem.SaveFile("Image|*.PNG;", ".png");
            if (!string.IsNullOrEmpty(imageSave))
            {
                using (var ms = new MemoryStream(Image))
                {
                    using (var image = System.Drawing.Image.FromStream(ms))
                    {
                        image.Save(imageSave, ImageFormat.Png);
                    }
                }
            }

            var jsonSave = fileSystem.SaveFile("Metadata|*.JSON", ".json");
            if (!string.IsNullOrEmpty(jsonSave))
            {
                File.WriteAllText(jsonSave, Json);
            }
        }

        private bool CanSave() => Image.Length > 0 && !string.IsNullOrEmpty(Json);

        private Layer? GetSelectedLayer()
        {
            return sessionProvider.Session().Layers.FirstOrDefault(l => l.Name == selectedLayer);
        }

        private void OnSuccess()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateSelection();
            });
        }

        private class ImageGenerationProcess : IGenerationProcess
        {
            public event Action? ProcessComplete;

            public event Action? ErrorFound;

            public event Action<double>? ProgressMade;

            public DateTime Start { get; } = DateTime.Now;

            public CancellationTokenSource TokenSource { get; } = new();

            public void Cancel()
            {
                TokenSource.Cancel();
            }

            public void RespectCheckpoint()
            {
                TokenSource.Token.ThrowIfCancellationRequested();
            }

            public void SetPaused(bool paused)
            {
                throw new NotImplementedException();
            }

            public void Complete()
            {
                Cancel();
                ProcessComplete?.Invoke();
            }
        }
    }
}
