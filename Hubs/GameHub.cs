using BreakingBank.Models.SaveGame;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BreakingBank.Hubs
{
    [Authorize]
    public class GameHub : Hub<IGameClient>
    {
        private readonly ILogger<GameHub> _logger;
        private readonly ISaveGameService _saveGameService;

        public GameHub(ILogger<GameHub> logger, ISaveGameService saveGameService) 
        {
            _logger = logger;
            _saveGameService = saveGameService;
        }

        public override Task OnConnectedAsync()
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            _logger.LogInformation($"{username} connected to GameHub");
            _saveGameService.Register(username);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            _logger.LogInformation($"{username} disconnected from GameHub");
            _saveGameService.Unregister(username);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task AddMoney(int amount)
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            _logger.LogInformation($"{username} wants to add money");
            SaveGame saveGame = _saveGameService.GetSaveGame(username);
            saveGame.Economy.Money.Value = saveGame.Economy.Money.Value + amount >= 0 ? saveGame.Economy.Money.Value + amount : 0;
           // await Clients.All.ReceiveDirtySaveGame(saveGame.DirtyData, saveGame.Serialize(true));
        }

        public async Task AddPassiveIncome(int amount)
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            SaveGame saveGame = _saveGameService.GetSaveGame(username);
            saveGame.Economy.PassiveIncome.Value = saveGame.Economy.PassiveIncome.Value + amount >= 0 ? saveGame.Economy.PassiveIncome.Value + amount : 0;
            //await Clients.All.ReceiveDirtySaveGame(saveGame.DirtyData, saveGame.Serialize(true));
        }

        public async Task GetDirtyData()
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            SaveGame saveGame = _saveGameService.GetSaveGame(username);
            await Clients.All.ReceiveDirtyData(saveGame.DirtyData);
            saveGame.ClearDirtyData();
        }

        public async Task GetSaveGame()
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            SaveGame saveGame = _saveGameService.GetSaveGame(username);
            await Clients.All.ReceiveSaveGame(saveGame);
        }
    }
}
