using Prism.Regions;
using Vortex.GenerativeArtSuite.Common.ViewModels;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Base
{
    public abstract class NavigationAware : NotifyPropertyChanged, INavigationAware
    {
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}
