using Prism.Ioc;
using Vortex.GenerativeArtSuite.Create.ViewModels;

namespace Vortex.GenerativeArtSuite.Create
{
    internal static class ContainerRegistration
    {
        internal static void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.RegisterSingleton<MainWindowVM>();
        }
    }
}
