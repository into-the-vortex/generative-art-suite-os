using System;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    internal class NavigationLock : INavigationLock
    {
        private bool locked;

        public event Action<bool>? LockChanged;

        public void Capture()
        {
            if(!locked)
            {
                locked = true;
                LockChanged?.Invoke(locked);
            }
        }

        public void Release()
        {
            if (locked)
            {
                locked = false;
                LockChanged?.Invoke(locked);
            }
        }
    }
}
