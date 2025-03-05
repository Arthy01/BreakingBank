using BreakingBank.Models.SaveGame;

namespace BreakingBank.Services
{
    public interface ISaveGameService
    {
        SaveGame GetActiveSaveGame(string player);

        void SaveGame(string player, SaveGame saveGame);

        void Register(string player);
        void Unregister(string player);
    }
}
