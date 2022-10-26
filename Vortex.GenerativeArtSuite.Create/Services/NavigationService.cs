using Prism.Mvvm;
using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    internal class NavigationService : BindableBase, INavigationService
    {
        public const string MainRegion = nameof(MainRegion);
        public const string SessionRegion = nameof(SessionRegion);

        public const string Home = nameof(Home);
        public const string Session = nameof(Session);
        public const string Layers = nameof(Layers);
        public const string Traits = nameof(Traits);
        public const string Generate = nameof(Generate);
        public const string Settings = nameof(Settings);

        private readonly IRegionManager regionManager;
        private IRegion? sessionRegion;
        private IRegion? mainRegion;
        private string? currentView;

        public NavigationService(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public string? CurrentView
        {
            get => currentView;
            set => SetProperty(ref currentView, value);
        }

        public void GoHome()
        {
            if (mainRegion is null && !TryGetMainRegion())
            {
                return;
            }

            mainRegion.RequestNavigate(Home, OnNavigation);
        }

        public void OpenSession(Session session)
        {
            if (mainRegion is null && !TryGetMainRegion())
            {
                return;
            }

            var parameters = new NavigationParameters
            {
                { nameof(Session), session },
            };
            mainRegion.RequestNavigate(Session, OnNavigation, parameters);
        }

        public void NavigateTo(string? tag, NavigationParameters? parameters = null)
        {
            if (tag is null || (sessionRegion is null && !TryGetSessionRegion()))
            {
                return;
            }

            sessionRegion.RequestNavigate(tag, OnNavigation, parameters);
        }

        private bool TryGetMainRegion()
        {
            if (mainRegion is null && regionManager.Regions.ContainsRegionWithName(MainRegion))
            {
                mainRegion = regionManager.Regions[MainRegion];
                mainRegion.NavigationService.Navigated += NavigationService_Navigated;
            }

            return mainRegion != null;
        }

        private bool TryGetSessionRegion()
        {
            if (sessionRegion is null && regionManager.Regions.ContainsRegionWithName(SessionRegion))
            {
                sessionRegion = regionManager.Regions[SessionRegion];
                sessionRegion.NavigationService.Navigated += NavigationService_Navigated;
            }

            return sessionRegion != null;
        }

        private void NavigationService_Navigated(object? sender, RegionNavigationEventArgs e)
        {
            UpdateCurrentView(e.NavigationContext);
        }

        private void OnNavigation(NavigationResult e)
        {
            UpdateCurrentView(e.Context);
        }

        private void UpdateCurrentView(NavigationContext e)
        {
            CurrentView = e.Uri.OriginalString;
        }
    }
}
