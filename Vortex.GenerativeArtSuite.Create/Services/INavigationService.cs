using Prism.Regions;
using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface INavigationService
    {
        void GoHome();

        void OpenSession(Session session);

        void NavigateTo(string? tag, NavigationParameters? parameters = null);
    }
}
