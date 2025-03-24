namespace BreakingBank.Models
{
    public struct AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UserID { get; set; }
        public User? User { get; set; }
    }
}
