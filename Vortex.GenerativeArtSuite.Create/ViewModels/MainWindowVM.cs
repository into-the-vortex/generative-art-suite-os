using System.Windows.Input;
using Prism.Commands;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class MainWindowVM
    {
        public MainWindowVM(INavigationService navigationService, ChangeThemeVM changeTheme)
        {
            ChangeThemeVM = changeTheme;
            OnLoaded = new DelegateCommand(navigationService.GoHome);
        }

        public ICommand OnLoaded { get; }

        public ChangeThemeVM ChangeThemeVM { get; }
    }
}
