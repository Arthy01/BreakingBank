using BreakingBank.Models;
using BreakingBank.Models.SaveGame;

namespace BreakingBank.Services
{
    public interface ISaveGameService
    {
        Task<string> CreateSaveGame(User owner, string name);
        Task<bool> UpdateSaveGame(SaveGame saveGame);
        Task<SaveGame?> GetSaveGame(string saveGameID);
        Task<List<SaveGame>> GetOwnedSaveGames(User user);
        Task<bool> SaveGameIDExists(string saveGameID);

        Task<bool> DeleteSaveGame(string saveGameID);

        /*

        SaveGame GetActiveSaveGame(string player);

        void SaveGame(string player, SaveGame saveGame);

        void Register(string player);
        void Unregister(string player);

        */
    }
}
