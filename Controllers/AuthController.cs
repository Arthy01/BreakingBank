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
        public IActionResult Login([FromBody] AuthRequest request)
        {
            // TODO
            // UserID needs to be the real userID not just the username

            if (request.Username == "admin" && request.Password == "password123")
            {
                var token = _jwtTokenService.GenerateToken(request.Username, "1");
                _logger.LogInformation("Issued Token: " + token);
                return Ok(new { Token = token });
            }

            if (request.Username == "test1" && request.Password == "password123")
            {
                var token = _jwtTokenService.GenerateToken(request.Username, "2");
                _logger.LogInformation("Issued Token: " + token);
                return Ok(new { Token = token });
            }

            if (request.Username == "test2" && request.Password == "password123")
            {
                var token = _jwtTokenService.GenerateToken(request.Username, "3");
                _logger.LogInformation("Issued Token: " + token);
                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Ungültige Anmeldeinformationen" });
        }
    }
}
