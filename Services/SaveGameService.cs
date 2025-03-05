using BreakingBank.Models.SaveGame;
using System.Collections.Concurrent;

namespace BreakingBank.Services
{
    public class SaveGameService : ISaveGameService
    {
        private readonly ILogger<SaveGameService> _logger;

        private static ConcurrentDictionary<string, SaveGame> _activeSaveGames = new();

        public SaveGameService(ILogger<SaveGameService> logger) 
        {
            _logger = logger;
        }

        public SaveGame GetActiveSaveGame(string player)
        {
            return _activeSaveGames[player];
        }

        public void Register(string player)
        {
            _activeSaveGames[player] = new SaveGame();
            _logger.LogInformation($"Registered {player} in SaveGameService");
        }

        public void SaveGame(string player, SaveGame saveGame)
        {
            _activeSaveGames[player] = saveGame;
        }

        public void Unregister(string player)
        {
            _activeSaveGames.TryRemove(player, out _);
            _logger.LogInformation($"Unregistered {player} in SaveGameService");
        }
    }
}
