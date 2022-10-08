namespace Vortex.GenerativeArtSuite.Create.Models
{
    public class Session
    {
        public Session(SessionSettings settings)
        {
            Settings = settings;
        }

        public SessionSettings Settings { get; }
    }
}
