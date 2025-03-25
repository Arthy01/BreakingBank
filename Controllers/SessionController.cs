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

        [HttpPost("create")]
        public async Task<ActionResult<string>> CreateSession(string saveGameID)
        {
            User user = Models.User.GetByClaims(User);
            
            if (!await _saveGameService.SaveGameIDExists(saveGameID))
            {
                return BadRequest($"Save game with ID {saveGameID} does not exist!");
            }

            if (!_sessionService.CreateSession(user, saveGameID, out string msg))
            {
                return BadRequest(msg);
            }

            await JoinSession(saveGameID);

            return Ok(msg);
        }

        [HttpPost("join")]
        public async Task<ActionResult<string>> JoinSession(string saveGameID)
        {
            User user = Models.User.GetByClaims(User);

            if (!await _saveGameService.SaveGameIDExists(saveGameID))
            {
                return BadRequest($"Save game with ID {saveGameID} does not exist!");
            }

            if(!_sessionService.JoinSession(user, saveGameID, out string msg))
            {
                return BadRequest(msg);
            }

            return Ok(msg);
        }

        [HttpPost("leave")]
        public ActionResult<string> LeaveSession()
        {
            User user = Models.User.GetByClaims(User);

            if (!_sessionService.LeaveSession(user, out string msg))
            {
                return BadRequest(msg);
            }

            return Ok(msg);
        }

        [HttpPost("close")]
        public ActionResult<string> CloseSession()
        {
            User user = Models.User.GetByClaims(User);

            if (!_sessionService.CloseSession(user, out string msg))
            {
                return BadRequest(msg);
            }

            return Ok(msg);
        }

        [HttpGet("activeSession")]
        public ActionResult<string> GetActiveSession()
        {
            Session? session = _sessionService.GetUserConnectedSession(Models.User.GetByClaims(User));

            if (session == null)
                return BadRequest("User has not entered a session yet");

            return Ok(session);
        }

        [HttpGet("sessions")]
        public ActionResult GetAllSessions()
        {
            return Ok(_sessionService.GetAllActiveSessions());
        }

        [HttpPost("save")]
        public ActionResult SaveSession()
        {
            User user = Models.User.GetByClaims(User);

            if (!_sessionService.SaveSession(user, out string msg))
                return BadRequest(msg);

            return Ok(msg);
        }
    }
}
