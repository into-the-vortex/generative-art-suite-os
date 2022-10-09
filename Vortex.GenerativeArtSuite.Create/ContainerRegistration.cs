using ModernWpf;
using Prism.Ioc;
using Prism.Regions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels;
using Vortex.GenerativeArtSuite.Create.Views.Pages;

namespace Vortex.GenerativeArtSuite.Create
{
    internal static class ContainerRegistration
    {
        internal static void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ThemeManager>(() => ThemeManager.Current);

            containerRegistry.RegisterSingleton<IFileSystem, LocalFileSystem>();
            containerRegistry.RegisterSingleton<IRegionManager, RegionManager>();
            containerRegistry.RegisterSingleton<INavigationService, NavigationService>();

            containerRegistry.RegisterSingleton<ChangeThemeVM>();
            containerRegistry.RegisterSingleton<MainWindowVM>();

            containerRegistry.RegisterForNavigation<HomePage, HomeVM>(NavigationService.Home);
            containerRegistry.RegisterForNavigation<SessionPage, SessionVM>(NavigationService.Session);
            containerRegistry.RegisterForNavigation<LayersPage, LayersVM>(NavigationService.Layers);
            containerRegistry.RegisterForNavigation<TraitsPage, TraitsVM>(NavigationService.Traits);
            containerRegistry.RegisterForNavigation<GeneratePage, GenerateVM>(NavigationService.Generate);
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsVM>(NavigationService.Settings);
        }
    }
}
