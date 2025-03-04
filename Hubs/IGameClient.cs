using BreakingBank.Models.SaveGame;

namespace BreakingBank.Hubs
{
    public interface IGameClient
    {
        Task ReceiveDirtyData(IReadOnlyDictionary<string, object> dirtyData);
        Task ReceiveSaveGame(SaveGame saveGame);
    }
}
