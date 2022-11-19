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
        private object result;

        public CharacterBuilderVM(ISessionProvider sessionProvider, IFileSystem fileSystem)
        {
            this.sessionProvider = sessionProvider;
            this.fileSystem = fileSystem;
            Save = new DelegateCommand(OnSave, CanSave)
                .ObservesProperty(() => Result);
            Randomise = new DelegateCommand(OnRandomise);

            result = new CharacterBuilderLoadingVM();
        }

        public object Result
        {
            get => result;
            set => SetProperty(ref result, value);
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
            var healthCheck = sessionProvider.Session().HealthCheck();
            if (!string.IsNullOrEmpty(healthCheck))
            {
                OnError(healthCheck);
                return;
            }

            var layer = GetSelectedLayer();

            if (layer is not null && generation is not null)
            {
                generation = sessionProvider
                    .Session()
                    .CreateGeneration(0, generation.SwapTrait(layer, trait).BuildOrder);

                Redraw();
            }
        }

        private void Redraw()
        {
            Result = new CharacterBuilderLoadingVM();
            StartGeneration();
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
                process.ProcessError -= OnError;
                process.Cancel();
                process = null;
            }

            process = new ImageGenerationProcess();
            process.ProcessComplete += OnSuccess;
            process.ProcessError += OnError;

            Task.Run(() =>
            {
                var settings = sessionProvider.Session().GenerationSettings;

                try
                {
                    using (var image = generation.GenerateImage(process))
                    {
                        using (var stream = new MemoryStream())
                        {
                            image.Save(stream, ImageFormat.Png);
                            Result = new CharacterBuilderResultVM(
                                JsonConvert.SerializeObject(generation.GenerateMetadata(settings), Formatting.Indented),
                                stream.ToArray());
                        }
                    }

                    process.Complete();
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    process.Error(e.Message);
                }
            });
        }

        private void OnSuccess()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateSelection();
            });
        }

        private void OnError(string error)
        {
            Result = new CharacterBuilderErrorVM(error);
        }

        private void OnRandomise()
        {
            var healthCheck = sessionProvider.Session().HealthCheck();
            if (!string.IsNullOrEmpty(healthCheck))
            {
                OnError(healthCheck);
                return;
            }
            generation = sessionProvider.Session().CreateRandomGeneration(0);
            Redraw();
        }

        private void OnSave()
        {
            if (Result is CharacterBuilderResultVM resultVM)
            {
                var imageSave = fileSystem.SaveFile("Image|*.PNG;", ".png");
                if (!string.IsNullOrEmpty(imageSave))
                {
                    using (var ms = new MemoryStream(resultVM.Image))
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
                    File.WriteAllText(jsonSave, resultVM.Json);
                }
            }
        }

        private bool CanSave() => Result is CharacterBuilderResultVM;

        private Layer? GetSelectedLayer()
        {
            return sessionProvider.Session().Layers.FirstOrDefault(l => l.Name == selectedLayer);
        }

        private class ImageGenerationProcess : IGenerationProcess
        {
            public event Action? ProcessComplete;

            public event Action<string>? ProcessError;

            public CancellationTokenSource TokenSource { get; } = new();

            public void Complete()
            {
                Cancel();
                ProcessComplete?.Invoke();
            }

            public void Error(string error)
            {
                ProcessError?.Invoke(error);
            }

            public void Cancel()
            {
                TokenSource.Cancel();
            }

            public void RespectCheckpoint()
            {
                TokenSource.Token.ThrowIfCancellationRequested();
            }
        }
    }
}
