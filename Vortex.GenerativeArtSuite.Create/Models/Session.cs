namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class Session
    {
        public Session(string name, SessionSettings settings)
        {
            Name = name;
            Settings = settings;
        }

        public string Name { get; }

        public SessionSettings Settings { get; }
    }
}
