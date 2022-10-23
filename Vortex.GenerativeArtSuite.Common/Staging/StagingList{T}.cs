using System;
using System.Collections.Generic;
using Vortex.GenerativeArtSuite.Common.Extensions;

namespace Vortex.GenerativeArtSuite.Common.Staging
{
    public sealed class StagingList<T> : List<T>
    {
        private readonly List<T> reference;
        private readonly Action? onApply;

        public StagingList(List<T> reference, Action? onApply = null)
        {
            this.reference = reference;
            this.onApply = onApply;

            AddRange(reference);
        }

        public bool IsDirty => !reference.ContainsEqualItems(this);

        public void Apply()
        {
            if(IsDirty)
            {
                reference.Clear();
                reference.AddRange(this);
                onApply?.Invoke();
            }
        }

        public void Revert()
        {
            Clear();
            AddRange(reference);
        }
    }
}
