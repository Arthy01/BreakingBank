using System.Security.Claims;

namespace BreakingBank.Models
{
    public struct User
    {
        public int ID { get; }
        public string Username { get; } = string.Empty;

        public static User GetByClaims(ClaimsPrincipal? claims)
        {
            string username = claims?.Identity?.Name!;
            int userID = int.Parse(claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            return new User(userID, username);
        }

        public User(int id, string username)
        {
            ID = id;
            Username = username;
        }

        public override bool Equals(object? obj)
        {
            if (obj is User other)
            {
                return ID == other.ID;
            }

            return false;
        }

        public override int GetHashCode() => ID.GetHashCode();
    }
}
