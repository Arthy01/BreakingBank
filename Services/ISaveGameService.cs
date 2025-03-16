using BreakingBank.Models;
using BreakingBank.Models.SaveGame;

namespace BreakingBank.Services
{
    public interface ISaveGameService
    {
        string CreateSaveGame(User owner, string name);
        void UpdateSaveGame(SaveGame saveGame);
        SaveGame? GetSaveGame(string saveGameID);
        (List<SaveGame> ownedSaveGames, List<SaveGame> coOwnedSaveGames) GetAllSaveGames(User user);
        List<SaveGame> GetAllSaveGames();
        bool SaveGameIDExists(string saveGameID);

        /*

        SaveGame GetActiveSaveGame(string player);

        void SaveGame(string player, SaveGame saveGame);

        void Register(string player);
        void Unregister(string player);

        */
    }
}
