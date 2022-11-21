using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Home
{
    public class RecentSessionVM : BindableBase
    {
        private readonly INavigationService navigationService;
        private readonly ISessionManager sessionManager;
        private readonly RecentSession model;
        private bool busy;

        public RecentSessionVM(RecentSession model, ISessionManager sessionManager, INavigationService navigationService)
        {
            this.model = model;
            this.sessionManager = sessionManager;
            this.navigationService = navigationService;

            sessionManager.OnBusyChanged += busy =>
            {
                Busy = busy;
            };

            OnClick = new DelegateCommand(OpenSession, CanOpenSession)
                .ObservesProperty(() => Busy);
        }

        public string Name => model.Name;

        public string Created => $"{Strings.Created} {model.Created.ToShortTimeString()} {model.Created.ToShortDateString()}";

        public string Modified => $"{Strings.Modified} {model.Modified.ToShortTimeString()} {model.Modified.ToShortDateString()}";

        public bool Busy
        {
            get => busy;
            set => SetProperty(ref busy, value);
        }

        public ICommand OnClick { get; }

        private void OpenSession()
        {
            Task.Run(async () =>
            {
                try
                {
                    await sessionManager.OpenExistingSession(model.Name);
                    navigationService.NavigateTo(NavigationService.Layers);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private bool CanOpenSession() => !busy;
    }
}
