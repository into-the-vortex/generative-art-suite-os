using System;
using System.Globalization;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generating
{
    public class GenerationVM : BindableBase
    {
        private readonly ISessionProvider sessionProvider;
        private readonly INavigationLock navigationLock;
        private readonly DebugConsole console = new();
        private readonly Action refreshButtons;

        private string remaining = string.Empty;
        private string elapsed = string.Empty;
        private IGenerationProcess? process;
        private double progress;
        private bool running;
        private bool paused;
        private bool error;

        public GenerationVM(ISessionProvider sessionProvider, INavigationLock navigationLock)
        {
            this.sessionProvider = sessionProvider;
            this.navigationLock = navigationLock;

            var start = new DelegateCommand(OnStart);
            var stop = new DelegateCommand(OnStop, CanStop);

            UpdateTimes();

            refreshButtons = new Action(() =>
            {
                start.RaiseCanExecuteChanged();
                stop.RaiseCanExecuteChanged();
            });

            StartGeneration = start;
            StopGeneration = stop;

            ConsoleVM = new DebugConsoleVM(console, 3);
        }

        public double Progress
        {
            get => progress;
            set => SetProperty(ref progress, value);
        }

        public bool Error
        {
            get => error;
            set => SetProperty(ref error, value);
        }

        public bool Paused
        {
            get => paused;
            set => SetProperty(ref paused, value, () => process?.SetPaused(value));
        }

        public bool Running
        {
            get => running;
            set => SetProperty(ref running, value);
        }

        public string Elapsed
        {
            get => elapsed;
            set => SetProperty(ref elapsed, $"{Strings.Elapsed} {value}");
        }

        public string Remaining
        {
            get => remaining;
            set => SetProperty(ref remaining, $"{Strings.Remaining} {value}");
        }

        public DebugConsoleVM ConsoleVM { get; }

        public ICommand StartGeneration { get; }

        public ICommand StopGeneration { get; }

        public void Reset()
        {
            ResetDisplay();
            console.Clear();
        }

        private void OnStart()
        {
            if (process is null)
            {
                navigationLock.Capture();
                CreateProcess();
                Reset();
            }
            else
            {
                Paused = !Paused;
                Running = !paused;
            }
        }

        private void OnStop()
        {
            process?.Cancel();
            RemoveProcess();
            ResetDisplay();

            navigationLock.Release();
        }

        private bool CanStop() => process is not null;

        private void UpdateTimes()
        {
            if (process is null)
            {
                var timespan = TimeSpan.Zero;

                Elapsed = timespan.ToString("hh':'mm':'ss", CultureInfo.CurrentCulture);
                Remaining = timespan.ToString("hh':'mm':'ss", CultureInfo.CurrentCulture);
            }
            else
            {
                var elapsed = DateTime.Now - process.Start;
                Elapsed = elapsed.ToString("hh':'mm':'ss", CultureInfo.CurrentCulture);

                var remaining = progress == 0 ? TimeSpan.MaxValue : elapsed / progress * (100 - progress);
                Remaining = remaining.ToString("hh':'mm':'ss", CultureInfo.CurrentCulture);
            }
        }

        private void OnSuccess()
        {
            RemoveProcess();
            ResetDisplay();
        }

        private void OnProgress(double progress)
        {
            Progress = progress;
            UpdateTimes();
        }

        private void OnError()
        {
            Error = true;

            RemoveProcess();
            refreshButtons();
            UpdateTimes();

            navigationLock.Release();
        }

        private void CreateProcess()
        {
            process = Generator.GenerateFor(sessionProvider.Session(), console);
            process.ProcessComplete += OnSuccess;
            process.ProgressMade += OnProgress;
            process.ErrorFound += OnError;

            Running = true;
        }

        private void RemoveProcess()
        {
            if (process is not null)
            {
                process.ProcessComplete -= OnSuccess;
                process.ProgressMade -= OnProgress;
                process.ErrorFound -= OnError;
                process = null;

                Running = false;
            }
        }

        private void ResetDisplay()
        {
            refreshButtons();
            UpdateTimes();
            Error = false;
            Paused = false;
            Progress = 0;
        }
    }
}
