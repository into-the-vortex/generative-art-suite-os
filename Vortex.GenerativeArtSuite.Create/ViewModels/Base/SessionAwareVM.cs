using System;
using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Base
{
    public abstract class SessionAwareVM : NavigationAware
    {
        private Session? currentSession;

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters[nameof(Session)] is Session session)
            {
                currentSession = session;
            }
        }

        protected Session Session()
        {
            if (currentSession is null)
            {
                throw new InvalidOperationException("Session is not set");
            }

            return currentSession;
        }
    }
}
