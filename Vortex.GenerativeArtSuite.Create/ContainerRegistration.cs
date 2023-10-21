using ModernWpf;
using Prism.Ioc;
using Prism.Regions;
using Vortex.GenerativeArtSuite.Common.ViewModels;
using Vortex.GenerativeArtSuite.Create.Services;
using Vortex.GenerativeArtSuite.Create.ViewModels;
using Vortex.GenerativeArtSuite.Create.ViewModels.Base;
using Vortex.GenerativeArtSuite.Create.ViewModels.Generating;
using Vortex.GenerativeArtSuite.Create.ViewModels.Home;
using Vortex.GenerativeArtSuite.Create.ViewModels.Layers;
using Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Create;
using Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Delete;
using Vortex.GenerativeArtSuite.Create.ViewModels.Layers.Edit;
using Vortex.GenerativeArtSuite.Create.ViewModels.Settings;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Create;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Delete;
using Vortex.GenerativeArtSuite.Create.ViewModels.Traits.Edit;
using Vortex.GenerativeArtSuite.Create.Views.Dialogs;
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
            containerRegistry.RegisterSingleton<INavigationLock, NavigationLock>();

            containerRegistry.RegisterManySingleton<SessionManager>(typeof(ISessionManager), typeof(ISessionProvider));
            containerRegistry.RegisterSingleton<ChangeThemeVM>();
            containerRegistry.RegisterSingleton<MainWindowVM>();

            containerRegistry.RegisterForNavigation<HomePage, HomeVM>(NavigationService.Home);
            containerRegistry.RegisterForNavigation<LayersPage, LayersVM>(NavigationService.Layers);
            containerRegistry.RegisterForNavigation<TraitsPage, LayerSelectorVM>(NavigationService.Traits);
            containerRegistry.RegisterForNavigation<GeneratePage, GenerateVM>(NavigationService.Generate);
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsVM>(NavigationService.Settings);

            containerRegistry.RegisterDialog<CreateLayerDialog, CreateLayerDialogVM>(DialogVM.CreateLayerDialog);
            containerRegistry.RegisterDialog<EditLayerDialog, EditLayerDialogVM>(DialogVM.EditLayerDialog);
            containerRegistry.RegisterDialog<DeleteLayerDialog, DeleteLayerDialogVM>(DialogVM.DeleteLayerDialog);

            containerRegistry.RegisterDialog<CreateTraitDialog, CreateTraitDialogVM>(DialogVM.CreateTraitDialog);
            containerRegistry.RegisterDialog<EditTraitDialog, EditTraitDialogVM>(DialogVM.EditTraitDialog);
            containerRegistry.RegisterDialog<DeleteTraitDialog, DeleteTraitDialogVM>(DialogVM.DeleteTraitDialog);

            containerRegistry.RegisterDialog<ConfirmSaveDialog, ConfirmSaveDialogVM>(DialogVM.ConfirmSaveDialog);
        }
    }
}
