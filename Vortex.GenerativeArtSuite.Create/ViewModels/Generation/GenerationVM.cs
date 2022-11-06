using System;
using System.Globalization;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;
using Console = Vortex.GenerativeArtSuite.Common.Models.Console;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generation
{
    public class GenerationVM : SessionAwareVM
    {
        private readonly Action refreshButtons;
        private readonly Console console = new();

        private IGenerationProcess? process;
        private Session? session;
        private double progress;
        private bool error;
        private bool paused;
        private string elapsed;
        private string remaining;

        public GenerationVM()
        {
            var start = new DelegateCommand(OnStart, CanStart);
            var restart = new DelegateCommand(OnRestart, CanRestart);
            var stop = new DelegateCommand(OnStop, CanStop);

            var timespan = TimeSpan.Zero;
            elapsed = timespan.ToString("hh':'mm':'ss", CultureInfo.CurrentCulture);
            remaining = timespan.ToString("hh':'mm':'ss", CultureInfo.CurrentCulture);

            refreshButtons = new Action(() =>
            {
                start.RaiseCanExecuteChanged();
                restart.RaiseCanExecuteChanged();
                stop.RaiseCanExecuteChanged();
            });

            StartGeneration = start;
            RestartGeneration = restart;
            StopGeneration = stop;

            ConsoleVM = new ConsoleVM(console, 3);
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
            set => SetProperty(ref paused, value);
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

        public ConsoleVM ConsoleVM { get; }

        public ICommand StartGeneration { get; }

        public ICommand RestartGeneration { get; }

        public ICommand StopGeneration { get; }

        public void ConnectSession(Session session)
        {
            if (this.session != session)
            {
                this.session = session;

                // TODO: stuff
            }
        }

        private void OnStart()
        {
            CreateProcess();
            refreshButtons();
            UpdateTimes();
            Reset();
        }

        private bool CanStart() => process is null && session is not null;

        private void OnRestart()
        {
        }

        private bool CanRestart() => false;

        private void OnStop()
        {
            RemoveProcess();
            refreshButtons();
            UpdateTimes();
            Reset();
        }

        private bool CanStop() => process is not null;

        private void OnSuccess()
        {

        }

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

                var remaining = progress == 0 ? TimeSpan.MaxValue : (elapsed * (100 / progress));
                Remaining = remaining.ToString("hh':'mm':'ss", CultureInfo.CurrentCulture);
            }
        }

        private void OnProgress(double progress)
        {
            Progress = progress;
            UpdateTimes();
        }

        private void OnError()
        {
            Error = true;
        }

        private void CreateProcess()
        {
            if (session is not null)
            {
                process = Generator.GenerateFor(session, console);
                process.ProcessComplete += OnSuccess;
                process.ProgressMade += OnProgress;
                process.ErrorFound += OnError;
            }
        }

        private void RemoveProcess()
        {
            if (process is not null)
            {
                process.Cancel();
                process.ProgressMade -= OnProgress;
                process.ErrorFound -= OnError;
                process = null;
            }
        }

        private void Reset()
        {
            Error = false;
            Paused = false;
            Progress = 0;
        }
    }
}
