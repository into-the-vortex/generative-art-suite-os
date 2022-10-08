using System;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    internal struct Gene
    {
        public Guid Identifier { get; }

        public bool Hereditary { get; set; }
    }
}
