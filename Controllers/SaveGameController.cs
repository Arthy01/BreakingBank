using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BreakingBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaveGameController : ControllerBase
    {
        private readonly ISaveGameService _saveGameService;

        public SaveGameController(ISaveGameService saveGameService)
        {
            _saveGameService = saveGameService;
        }

        [HttpPost("create")]
        public ActionResult<string> CreateSaveGame(string saveGameName)
        {
            Models.User user = Models.User.GetByClaims(User);

            string id = _saveGameService.CreateSaveGame(user, saveGameName);

            return Ok($"Save game created with id {id}");
        }
    }
}
