using BreakingBank.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BreakingBank.Services
{
    public class JWTService
    {
        private readonly ILogger<JWTService> _logger;
        private readonly JWTSettings _jwtSettings;

        public JWTService(ILogger<JWTService> logger, IOptions<JWTSettings> jwtSettings)
        {
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(string username, string userid)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims =
            [
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, userid)
            ];

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
