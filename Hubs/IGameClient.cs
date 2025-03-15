using BreakingBank.Models.SaveGame;

namespace BreakingBank.Hubs
{
    public interface IGameClient
    {
        Task ForceDisconnect();
        /*
        Task ReceiveSaveGame(SaveGame saveGame);
        Task ReceiveDirtyData(IReadOnlyDictionary<string, object> dirtyData);
        Task ReceiveTick(IReadOnlyDictionary<string, object> dirtyData);
        */
    }
}
