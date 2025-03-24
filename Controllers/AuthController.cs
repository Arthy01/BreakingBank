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
            string x = "{\"economy\":{\"paper\":187,\"wetMoney\":0,\"cartridges\":0,\"cleanMoney\":0,\"dirtyMoney\":0},\"metaData\":{\"id\":\"b8a3e7f1-ff3b-4c4b-85e4-544286f31d14\",\"name\":\"TestSaveGame\",\"ownerUserID\":1,\"coOwnerUserIDs\":[]},\"upgrades\":{\"upgrades\":[{\"name\":\"Test Upgrade\",\"level\":0,\"baseCost\":100,\"baseEffect\":1,\"description\":\"Test Description\",\"costIncrease\":50}]},\"processing\":{\"dryers\":{\"count\":0,\"maxCapacity\":0,\"usedCapacity\":0,\"currentClicks\":0,\"requiredClicks\":0},\"printers\":{\"count\":0,\"maxCapacity\":0,\"usedCapacity\":0,\"currentClicks\":0,\"requiredClicks\":0},\"washingMachines\":{\"count\":0,\"maxCapacity\":0,\"usedCapacity\":0,\"currentClicks\":0,\"requiredClicks\":0}}}";
            bool s = SaveGame.Parse(x, out SaveGame saveGame);
            Console.WriteLine(s);
            // TODO
            // UserID needs to be the real userID not just the username

            AuthResponse response = await _databaseHelper.AuthUser(request);

            if (!response.Success)
                return Unauthorized(new { Message = "Ungültige Anmeldeinformationen" });

            User user = response.User!.Value;
            string token = _jwtTokenService.GenerateToken(user.Username, user.ID.ToString());
            _logger.LogInformation("Issued Toke: " + token);
            return Ok(new { Token = token });

            /*
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
            */
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            await _databaseHelper.CreateUser(request.Username, request.Password);
            return Ok();
        }
    }
}
