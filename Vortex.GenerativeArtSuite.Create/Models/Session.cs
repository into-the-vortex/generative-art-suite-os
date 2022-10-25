using System.Collections.Generic;

namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class Session
    {
        public Session(string name, SessionSettings settings)
        {
            Name = name;
            Settings = settings;
            Layers = new List<Layer>();
        }

        public string Name { get; }

        public SessionSettings Settings { get; }

        public List<Layer> Layers { get; }
    }
}
