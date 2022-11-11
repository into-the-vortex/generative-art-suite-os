using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generating
{
    public class CharacterBuilderVM : BindableBase
    {
        private readonly ISessionProvider sessionProvider;
        private ImageGenerationProcess? process;
        private Generation? generation;
        private string? selectedLayer;
        private string? image;
        private string json = string.Empty;

        public CharacterBuilderVM(ISessionProvider sessionProvider)
        {
            this.sessionProvider = sessionProvider;

            var save = new DelegateCommand(OnSave, CanSave);
            save.ObservesProperty(() => Json);
            save.ObservesProperty(() => Image);

            Save = save;
            Randomise = new DelegateCommand(OnRandomise);
        }

        public string Json
        {
            get => json;
            set => SetProperty(ref json, value);
        }

        public string? Image
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

        public ObservableCollection<CharacterPathSelectorVM> PathCollections { get; } = new();

        public void Reset()
        {
            generation = sessionProvider.Session().CreateRandomGeneration(0);

            var used = new List<string>();
            PathCollections.Clear();
            foreach (var layer in sessionProvider.Session().Layers.Where(l => l.Paths.Any()))
            {
                foreach (var paths in layer.Paths)
                {
                    var con = string.Concat(paths.Options.OrderByDescending(p => p));
                    if (!used.Contains(con))
                    {
                        PathCollections.Add(new CharacterPathSelectorVM(
                            used.Count,
                            generation.ChosenPaths[used.Count],
                            paths.Options,
                            OnPathChanged));

                        used.Add(con);
                    }
                }
            }

            Layers.Clear();
            Layers.AddRange(sessionProvider.Session().Layers.Select(l => l.Name));

            SelectedLayer = Layers.FirstOrDefault();

            Redraw();
        }

        private void Redraw()
        {
            Image = null;
            Json = string.Empty;

            StartGeneration();
        }

        private void UpdateSelection()
        {
            Traits.Clear();

            var layer = sessionProvider.Session().Layers.FirstOrDefault(l => l.Name == selectedLayer);
            if (layer is not null)
            {
                Traits.AddRange(layer.Traits.Select(t => new CharacterTraitVM(t, OnTraitClicked)));
            }
        }

        private void OnTraitClicked(Trait trait)
        {
        }

        private void OnPathChanged(int index, string value)
        {
        }

        private void StartGeneration()
        {
            if (generation is null)
            {
                throw new InvalidOperationException();
            }

            if (process is not null)
            {
                process.ProcessComplete -= OnGenerationSuccess;
                process.Cancel();
                process = null;
            }

            process = new ImageGenerationProcess();
            process.ProcessComplete += OnGenerationSuccess;

            Task.Run(() =>
            {
                var settings = sessionProvider.Session().Settings;
                var output = settings.BuilderOutputFolder();

                generation.SaveGeneratedMetadata(process, output, settings);
                generation.SaveGeneratedImage(process, output);

                process.Complete();
            });
        }

        private void OnGenerationSuccess()
        {
            var output = sessionProvider.Session().Settings.BuilderOutputFolder();

            Json = File.ReadAllText(Path.Join(output, "0"));
            Image = Path.Join(output, "0.png");
        }

        private void OnRandomise()
        {
            generation = sessionProvider.Session().CreateRandomGeneration(1);
            Redraw();
        }

        private void OnSave()
        {

        }

        private bool CanSave() => File.Exists(Image) && !string.IsNullOrEmpty(Json);

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
