using System;
using System.Collections.Generic;

namespace Vortex.GenerativeArtSuite.Common.Staging
{
    public sealed class StagingProperty<T>
    {
        private readonly Action? onApply;
        private readonly Action<T> set;
        private readonly Func<T> get;
        private T value;

        public StagingProperty(Action<T> set, Func<T> get, Action? onApply = null)
        {
            this.onApply = onApply;
            this.set = set;
            this.get = get;
            value = get();
        }

        public T Value
        {
            get => value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(this.value, value))
                {
                    this.value = value;
                    IsDirty = !EqualityComparer<T>.Default.Equals(get(), value);
                }
            }
        }

        public bool IsDirty { get; private set; }

        public void Apply()
        {
            if (IsDirty)
            {
                set(Value);
                onApply?.Invoke();
            }
        }

        public void Revert()
        {
            Value = get();
        }
    }
}
