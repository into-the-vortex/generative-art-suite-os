using Prism.Ioc;
using Vortex.GenerativeArtSuite.Inspect.ViewModels;

namespace Vortex.GenerativeArtSuite.Inspect
{
    internal static class ContainerRegistration
    {
        internal static void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<MainWindowVM>();
        }
    }
}
