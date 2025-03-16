using BreakingBank.Models.SaveGame;
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

        [HttpGet("all")]
        public ActionResult GetAllSaveGames()
        {
            return Ok(_saveGameService.GetAllSaveGames());
        }

        [HttpGet]
        public ActionResult GetSaveGame(string saveGameID)
        {
            SaveGame? saveGame = _saveGameService.GetSaveGame(saveGameID);

            if (saveGame == null)
                return BadRequest($"SaveGame with ID {saveGameID} does not exist!");

            return Ok(saveGame);
        }
    }
}
