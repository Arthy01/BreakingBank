using BreakingBank.Helpers;
using BreakingBank.Models;
using BreakingBank.Models.SaveGame;

namespace BreakingBank.Services
{
    public class SaveGameServiceDatabase : ISaveGameService
    {
        private readonly DatabaseHelper _databaseHelper;

        public SaveGameServiceDatabase(DatabaseHelper databaseHelper) 
        {
            _databaseHelper = databaseHelper;
        }

        public async Task<string> CreateSaveGame(User owner, string name)
        {
            SaveGame saveGame = SaveGame.Create(owner, name);
            await _databaseHelper.CreateSaveGame(saveGame);

            return saveGame.MetaData.ID;
        }

        public async Task<bool> DeleteSaveGame(string saveGameID)
        {
            return await _databaseHelper.DeleteSaveGame(saveGameID);
        }

        public async Task<List<SaveGame>> GetOwnedSaveGames(User user)
        {
            List<SaveGame> ownedSaveGames = await _databaseHelper.GetOwnedSaveGamesByUser(user);

            return ownedSaveGames;
        }

        public async Task<SaveGame?> GetSaveGame(string saveGameID)
        {
            return await _databaseHelper.GetSaveGame(saveGameID);
        }

        public async Task<bool> SaveGameIDExists(string saveGameID)
        {
            return await _databaseHelper.SaveGameExists(saveGameID);
        }

        public async Task<bool> UpdateSaveGame(SaveGame saveGame)
        {
            return await _databaseHelper.UpdateSaveGame(saveGame);
        }
    }
}
