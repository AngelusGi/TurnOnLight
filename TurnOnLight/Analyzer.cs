namespace TurnOnLight
{
    public class Analyzer
    {
        public Intent Intent { get; set; }
        public Entity Entity { get; set; }
    }

    public enum Entity
    {
        Green,
        Yellow,
        AllLights
    }

    public enum Intent
    {
        LightOn,
        LightOff,
        None
    }
}
