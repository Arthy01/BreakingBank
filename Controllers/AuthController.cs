using BreakingBank.Models;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BreakingBank.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JWTService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(JWTService jwtTokenService, ILogger<AuthController> logger)
        {
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "admin" && request.Password == "password123")
            {
                var token = _jwtTokenService.GenerateToken(request.Username);
                _logger.LogInformation("Issued Token: " + token);
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Ungültige Anmeldeinformationen" });
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok(new { Message = "TEST" });
        }
    }
}
