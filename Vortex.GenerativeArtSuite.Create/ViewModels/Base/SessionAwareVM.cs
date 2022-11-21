using Prism.Regions;
using Prism.Services.Dialogs;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Base
{
    public abstract class SessionAwareVM : NavigationAwareVM
    {
        private readonly ISessionProvider sessionProvider;
        private readonly IDialogService dialogService;

        public SessionAwareVM(ISessionProvider sessionProvider, IDialogService dialogService)
        {
            this.sessionProvider = sessionProvider;
            this.dialogService = dialogService;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (sessionProvider.RequiresReset(GetType()))
            {
                ResetOnSessionChanged();
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            if (navigationContext.Uri.OriginalString == NavigationService.Home && sessionProvider.CanSaveSession())
            {
                dialogService.ShowDialog(DialogVM.ConfirmSaveDialog, ConfirmSaveCallback);
            }
        }

        protected abstract void ResetOnSessionChanged();

        private void ConfirmSaveCallback(IDialogResult dialogResult)
        {
            if(dialogResult.Result == ButtonResult.None)
            {
                
            }
        }
    }
}
