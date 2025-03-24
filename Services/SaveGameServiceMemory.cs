using BreakingBank.Models;
using BreakingBank.Models.SaveGame;
using System.Collections.Concurrent;

namespace BreakingBank.Services
{
    public class SaveGameServiceMemory : ISaveGameService
    {
        private readonly ILogger<SaveGameServiceMemory> _logger;

        private List<SaveGame> _createdSaveGames = [];

        public SaveGameServiceMemory(ILogger<SaveGameServiceMemory> logger) 
        {
            _logger = logger;
        }

        public string CreateSaveGame(User owner, string name)
        {
            SaveGame saveGame = SaveGame.Create(owner, name);
            _createdSaveGames.Add(saveGame);

            return saveGame.MetaData.ID;
        }

        public (List<SaveGame> ownedSaveGames, List<SaveGame> coOwnedSaveGames) GetAllSaveGames(User user)
        {
            throw new NotImplementedException();
        }

        public List<SaveGame> GetAllSaveGames()
        {
            return _createdSaveGames;
        }

        public SaveGame? GetSaveGame(string saveGameID)
        {
            return _createdSaveGames.Where(x => x.MetaData.ID == saveGameID).FirstOrDefault();
        }

        public void UpdateSaveGame(SaveGame saveGame)
        {
            throw new NotImplementedException();
        }

        public bool SaveGameIDExists(string saveGameID)
        {
            return _createdSaveGames.Find(x => x.MetaData.ID == saveGameID) != null;
        }

        public bool DeleteSaveGame(string saveGameID)
        {
            throw new NotImplementedException();
        }

        /*
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
        */
    }
}
