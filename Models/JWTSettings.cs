namespace BreakingBank.Models
{
    public class JWTSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 0;
    }
}
