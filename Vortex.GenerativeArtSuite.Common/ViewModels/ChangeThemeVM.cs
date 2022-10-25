using System.Windows.Input;
using ModernWpf;
using Prism.Commands;

namespace Vortex.GenerativeArtSuite.Common.ViewModels
{
    public class ChangeThemeVM
    {
        public ChangeThemeVM(ThemeManager themeManager)
        {
            ChangeTheme = new DelegateCommand(() =>
            {
                themeManager.ApplicationTheme =
                    themeManager.ActualApplicationTheme == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark;
            });
        }

        public ICommand ChangeTheme { get; set; }
    }
}
