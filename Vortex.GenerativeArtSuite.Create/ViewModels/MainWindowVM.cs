using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Services;

namespace Vortex.GenerativeArtSuite.Create.ViewModels
{
    public class MainWindowVM : BindableBase
    {
        private readonly INavigationService navigationService;

        private bool allowNavigation;
        private string? selectedTag;

        public MainWindowVM(
            INavigationLock navigationLock,
            INavigationService navigationService,
            ChangeThemeVM changeTheme)
        {
            this.navigationService = navigationService;
            ChangeThemeVM = changeTheme;
            OnLoaded = new DelegateCommand(() => SelectedTag = NavigationService.Home);

            navigationService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(INavigationService.CurrentView))
                {
                    SelectedTag = navigationService.CurrentView;
                }
            };

            navigationLock.LockChanged += (locked) =>
            {
                AllowNavigation = !locked;
            };
        }

        public ICommand OnLoaded { get; }

        public ChangeThemeVM ChangeThemeVM { get; }

        public bool AllowNavigation
        {
            get => allowNavigation;
            set => SetProperty(ref allowNavigation, value);
        }

        public string? SelectedTag
        {
            get => selectedTag;
            set => SetProperty(ref selectedTag, value, OnSelectedTagChanged);
        }

        private void OnSelectedTagChanged()
        {
            navigationService.NavigateTo(SelectedTag);
        }
    }
}
