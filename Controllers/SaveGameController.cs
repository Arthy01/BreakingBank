using BreakingBank.Models.SaveGame;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BreakingBank.Models;

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

        [HttpDelete]
        public ActionResult DeleteSaveGame(string saveGameID)
        {
            User user = Models.User.GetByClaims(User);

            SaveGame? saveGame = _saveGameService.GetSaveGame(saveGameID);

            if (saveGame == null)
                return NotFound($"SaveGame with ID {saveGameID} does not exist!");

            if (user.ID != saveGame.MetaData.OwnerUserID)
                return Unauthorized($"User is not authorized to delete the SaveGame with ID {saveGameID}. Only the owner of the SaveGame can delete it.");

            if (!_saveGameService.DeleteSaveGame(saveGameID))
                return BadRequest("Something went wrong...");

            return Ok();
        }
    }
}
