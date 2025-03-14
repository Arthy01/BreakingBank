using System.Security.Claims;

namespace BreakingBank.Models
{
    public struct User
    {
        public int ID { get; }
        public string Username { get; } = string.Empty;
        public string? ConnectionID { get; } = null;

        public static User GetByClaims(ClaimsPrincipal? claims, string? connectionID = null)
        {
            string username = claims?.Identity?.Name!;
            int userID = int.Parse(claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            return new User(userID, username, connectionID);
        }

        public User(int id, string username, string? connectionID = null)
        {
            ID = id;
            Username = username;
            ConnectionID = connectionID;
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
