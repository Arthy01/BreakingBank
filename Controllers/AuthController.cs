using BreakingBank.Helpers;
using BreakingBank.Models;
using BreakingBank.Models.SaveGame;
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
        private readonly DatabaseHelper _databaseHelper;

        public AuthController(JWTService jwtTokenService, ILogger<AuthController> logger, DatabaseHelper databaseHelper)
        {
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _databaseHelper = databaseHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            AuthResponse response = await _databaseHelper.AuthUser(request);

            if (!response.Success)
                return Unauthorized(new { Message = "Ungültige Anmeldeinformationen" });

            User user = response.User!.Value;
            string token = _jwtTokenService.GenerateToken(user.Username, user.ID.ToString());
            _logger.LogInformation("Issued Token: " + token);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            await _databaseHelper.CreateUser(request.Username, request.Password);
            return Ok();
        }
    }
}
