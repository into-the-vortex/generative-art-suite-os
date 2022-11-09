using Vortex.GenerativeArtSuite.Create.Models;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface ISessionProvider
    {
        void SaveSession();

        Session Session();
    }
}
