using System;

namespace Vortex.GenerativeArtSuite.Create.Models.Sessions
{
    public struct RecentSession
    {
        public string Name { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }
    }
}
