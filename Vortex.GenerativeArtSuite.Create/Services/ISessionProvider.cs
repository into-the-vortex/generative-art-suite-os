using System;
using System.Threading.Tasks;
using Vortex.GenerativeArtSuite.Create.Models.Sessions;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface ISessionProvider
    {
        event Action<bool>? OnBusyChanged;

        Task SaveSession(string commitMessage);

        Session Session();

        bool CanSaveSession();

        bool RequiresReset(Type viewer);
    }
}
