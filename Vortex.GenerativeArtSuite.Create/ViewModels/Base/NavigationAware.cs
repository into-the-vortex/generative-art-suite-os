using Prism.Mvvm;
using Prism.Regions;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Base
{
    public abstract class NavigationAware : BindableBase, INavigationAware
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
