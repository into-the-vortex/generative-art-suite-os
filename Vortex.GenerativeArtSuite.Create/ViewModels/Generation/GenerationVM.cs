using System;
using System.Globalization;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generation
{
    public class GenerationVM : SessionAwareVM
    {
        private readonly Action refreshButtons;
        private readonly DebugConsole console = new();

        private IGenerationProcess? process;
        private Session? session;
        private double progress;
        private bool error;
        private bool paused;
        private bool running;
        private string elapsed = string.Empty;
        private string remaining = string.Empty;

        public GenerationVM()
        {
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

        public void ConnectSession(Session session)
        {
            if (this.session != session)
            {
                this.session = session;

                RemoveProcess();
                refreshButtons();
                UpdateTimes();
                Reset();
            }
        }

        private void OnStart()
        {
            if (process is null)
            {
                CreateProcess();
                refreshButtons();
                UpdateTimes();
                Reset();

                console.Clear();
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
            refreshButtons();
            UpdateTimes();
            Reset();
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

        private void Reset()
        {
            Error = false;
            Paused = false;
            Progress = 0;
        }

        private void OnSuccess()
        {
            RemoveProcess();
            refreshButtons();
            UpdateTimes();
            Reset();
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
        }

        private void CreateProcess()
        {
            if (session is not null)
            {
                process = Generator.GenerateFor(session, console);
                process.ProcessComplete += OnSuccess;
                process.ProgressMade += OnProgress;
                process.ErrorFound += OnError;

                Running = true;
            }
        }

        private void RemoveProcess()
        {
            if (process is not null)
            {
                process.ProgressMade -= OnProgress;
                process.ErrorFound -= OnError;
                process = null;

                Running = false;
            }
        }
    }
}
