using System;
using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Create.Models;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generation
{
    public class GenerationVM : SessionAwareVM
    {
        private readonly Action refreshButtons;
        private IGenerationProcess? process;
        private Session? session;
        private double progress;
        private string? error;

        public GenerationVM()
        {
            var start = new DelegateCommand(OnStart, CanStart);
            var restart = new DelegateCommand(OnRestart, CanRestart);
            var stop = new DelegateCommand(OnStop, CanStop);

            refreshButtons = new Action(() =>
            {
                start.RaiseCanExecuteChanged();
                restart.RaiseCanExecuteChanged();
                stop.RaiseCanExecuteChanged();
            });

            StartGeneration = start;
            RestartGeneration = restart;
            StopGeneration = stop;
        }

        public double Progress
        {
            get => progress;
            set => SetProperty(ref progress, value);
        }

        public string? Error
        {
            get => error;
            set => SetProperty(ref error, value);
        }

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
        }

        private bool CanStop() => process is not null;

        private void OnSucess()
        {

        }

        private void OnProgress(double progress)
        {
            Progress = progress;
        }

        private void OnError(string error)
        {
            Error = error;
        }

        private void CreateProcess()
        {
            if (session is not null)
            {
                process = Generator.GenerateFor(session);
                process.ProcessComplete += OnSucess;
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
    }
}
