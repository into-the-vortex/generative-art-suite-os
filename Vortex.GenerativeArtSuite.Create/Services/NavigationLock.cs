using System;

namespace Vortex.GenerativeArtSuite.Create.Services
{
    internal class NavigationLock : INavigationLock
    {
        private bool locked;

        public event Action<bool>? LockChanged;

        public void Capture()
        {
            if(locked)
            {
                throw new InvalidOperationException("Lock already captured");
            }

            locked = true;
            LockChanged?.Invoke(locked);
        }

        public void Release()
        {
            if (!locked)
            {
                throw new InvalidOperationException("Lock already released");
            }

            locked = false;
            LockChanged?.Invoke(locked);
        }
    }
}
