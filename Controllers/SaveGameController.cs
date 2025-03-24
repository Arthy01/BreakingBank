using BreakingBank.Models.SaveGame;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BreakingBank.Models;
using BreakingBank.Helpers;

namespace BreakingBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaveGameController : ControllerBase
    {
        private readonly ISaveGameService _saveGameService;
        private readonly DatabaseHelper _databaseHelper;

        public SaveGameController(ISaveGameService saveGameService, DatabaseHelper databaseHelper)
        {
            _saveGameService = saveGameService;
            _databaseHelper = databaseHelper;
        }

        [HttpPost("create")]
        public async Task<ActionResult<string>> CreateSaveGame(string saveGameName)
        {
            Models.User user = Models.User.GetByClaims(User);

            string id = _saveGameService.CreateSaveGame(user, saveGameName);
            bool success = await _databaseHelper.CreateSaveGame(_saveGameService.GetSaveGame(id)!);

            
            return Ok($"Save game created with id {id} => " + success);
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
