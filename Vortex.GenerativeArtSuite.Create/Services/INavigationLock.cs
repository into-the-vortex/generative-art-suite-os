using System;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    public interface INavigationLock
    {
        event Action<bool>? LockChanged;

        void Capture();

        void Release();
    }
}
