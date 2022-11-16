using System;
using System.Collections.Generic;
using Vortex.GenerativeArtSuite.Create.Models.Settings;
using Vortex.GenerativeArtSuite.Create.Models.Traits;

namespace Vortex.GenerativeArtSuite.Create.Models.Layers
{
    public class PathingLayer : Layer
    {
        public PathingLayer()
            : this(string.Empty, true, new())
        {
        }

        public PathingLayer(string name, bool includeInDNA, List<Dependency> dependencies)
            : base(name, includeInDNA, dependencies)
        {
        }

        public override Trait CreateTrait(string name, string iconURI)
        {
            return new PathingTrait(name, iconURI);
        }

        public override void EnsureTraitsRemainValid(Session session)
        {
            _ = session; // TODO: inform session on any changes made.

            for (int i = 0; i < Traits.Count; i++)
            {
                if (Traits[i] is not PathingTrait)
                {
                    throw new NotImplementedException(); // This should not currently occur.
                }
            }
        }
    }
}
