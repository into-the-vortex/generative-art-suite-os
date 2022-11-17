using System;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface ISessionProvider
    {
        void SaveSession();

        Session Session();

        bool CanSaveSession();

        bool RequiresReset(Type viewer);
    }
}
