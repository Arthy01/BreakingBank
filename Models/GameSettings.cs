namespace BreakingBank.Models
{
    public class GameSettings
    {
        public float TickRate => 1000 / TickDelay;

        public int TickDelay { get; set; }
    }
}
