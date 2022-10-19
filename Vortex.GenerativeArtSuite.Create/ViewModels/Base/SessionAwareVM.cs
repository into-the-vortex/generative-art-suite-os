using System;
using Prism.Regions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Base
{
    public abstract class SessionAwareVM : NotifyPropertyChanged, INavigationAware
    {
        private Session? currentSession;

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
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
