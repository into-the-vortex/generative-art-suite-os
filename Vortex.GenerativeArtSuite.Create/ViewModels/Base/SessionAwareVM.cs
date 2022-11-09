using System;
using Newtonsoft.Json;
using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Base
{
    public abstract class SessionAwareVM : NavigationAwareVM, IConfirmNavigationRequest
    {
        private readonly ISessionProvider sessionProvider;
        private string lastSeenSession = string.Empty;

        public SessionAwareVM(ISessionProvider sessionProvider)
        {
            this.sessionProvider = sessionProvider;
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            bool okay = true;

            if(navigationContext.Uri.OriginalString == NavigationService.Home)
            {
                sessionProvider.SaveSession();
            }

            continuationCallback(okay);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            var dirtyComparison = JsonConvert.SerializeObject(sessionProvider.Session());
            if (lastSeenSession != dirtyComparison)
            {
                lastSeenSession = dirtyComparison;
                ResetOnSessionChanged();
            }
        }

        protected abstract void ResetOnSessionChanged();
    }
}
