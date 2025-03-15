using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BreakingBank.Models;

namespace BreakingBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly SessionService _sessionService;
        private readonly ISaveGameService _saveGameService;

        public SessionController(SessionService sessionService, ISaveGameService saveGameService)
        {
            _sessionService = sessionService;
            _saveGameService = saveGameService;
        }

        [HttpGet("username")]
        public ActionResult<string> GetUsername()
        {
            string? username = User?.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Benutzername nicht gefunden.");
            }

            return Ok(username);
        }

        [HttpPost("create")]
        public ActionResult<string> CreateSession(string saveGameID)
        {
            User user = Models.User.GetByClaims(User);
            
            if (!_saveGameService.SaveGameIDExists(saveGameID))
            {
                return BadRequest($"Save game with ID {saveGameID} does not exist!");
            }

            if (!_sessionService.CreateSession(user, saveGameID))
            {
                return BadRequest("Something went wrong...");
            }

            JoinSession(saveGameID);

            return Ok("Session created!");
        }

        [HttpPost("join")]
        public ActionResult<string> JoinSession(string saveGameID)
        {
            User user = Models.User.GetByClaims(User);

            if (!_saveGameService.SaveGameIDExists(saveGameID))
            {
                return BadRequest($"Save game with ID {saveGameID} does not exist!");
            }

            if(!_sessionService.JoinSession(user, saveGameID))
            {
                return BadRequest("Something went wrong...");
            }

            return Ok("Session joined!");
        }

        [HttpPost("leave")]
        public ActionResult<string> LeaveSession()
        {
            User user = Models.User.GetByClaims(User);

            if (!_sessionService.LeaveSession(user))
            {
                return BadRequest("User is not connected to a Session.");
            }

            return Ok("Session left!");
        }

        [HttpGet("activeSession")]
        public ActionResult<string> GetActiveSession()
        {
            Session? session = _sessionService.GetUserConnectedSession(Models.User.GetByClaims(User));

            if (session == null)
                return BadRequest("User has not entered a session yet");

            return Ok(session);
        }
    }
}
