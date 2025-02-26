using BreakingBank.Models;
using BreakingBank.Services;
using Microsoft.AspNetCore.Mvc;

namespace BreakingBank.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JWTService _jwtTokenService;

        public AuthController(JWTService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Beispielhafter Dummy-Login (hier solltest du deine echte User-Validierung einbauen)
            if (request.Username == "admin" && request.Password == "password123")
            {
                var token = _jwtTokenService.GenerateToken(request.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Ungültige Anmeldeinformationen" });
        }
    }
}
