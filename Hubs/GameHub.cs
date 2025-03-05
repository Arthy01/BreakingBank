using BreakingBank.Models.SaveGame;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BreakingBank.Hubs
{
    [Authorize]
    public class GameHub : Hub<IGameClient>
    {
        private readonly ILogger<GameHub> _logger;
        private readonly ISaveGameService _saveGameService;

        private static readonly ConcurrentDictionary<string, string> _connections = new();

        public GameHub(ILogger<GameHub> logger, ISaveGameService saveGameService) 
        {
            _logger = logger;
            _saveGameService = saveGameService;
        }

        public static string? GetUsernameFromConnectionId(string connectionId)
        {
            return _connections.TryGetValue(connectionId, out var username) ? username : null;
        }

        public static IReadOnlyDictionary<string, string> GetAllConnections()
        {
            return _connections;
        }

        public override Task OnConnectedAsync()
        {
            string? username = Context.User?.Identity?.Name ?? null;
            if (username == null)
            {
                Context.Abort();
                return base.OnConnectedAsync();
            }

            _logger.LogInformation($"{username} connected to GameHub");
            _connections[Context.ConnectionId] = username;
            _saveGameService.Register(username);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            _logger.LogInformation($"{username} disconnected from GameHub");
            _connections.TryRemove(Context.ConnectionId, out _);
            _saveGameService.Unregister(username);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task AddMoney(int amount)
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            _logger.LogInformation($"{username} wants to add money");
            SaveGame saveGame = _saveGameService.GetActiveSaveGame(username);
            saveGame.Economy.Money.Value = saveGame.Economy.Money.Value + amount >= 0 ? saveGame.Economy.Money.Value + amount : 0;
           // await Clients.All.ReceiveDirtySaveGame(saveGame.DirtyData, saveGame.Serialize(true));
        }

        public async Task AddPassiveIncome(float amount)
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            _logger.LogInformation($"{username} wants to add passive income");
            SaveGame saveGame = _saveGameService.GetActiveSaveGame(username);
            saveGame.Economy.PassiveIncome.Value = saveGame.Economy.PassiveIncome.Value + amount >= 0 ? saveGame.Economy.PassiveIncome.Value + amount : 0;
        }

        public async Task GetDirtyData()
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            SaveGame saveGame = _saveGameService.GetActiveSaveGame(username);
            await Clients.Client(Context.ConnectionId).ReceiveDirtyData(saveGame.DirtyData);
            saveGame.ClearDirtyData();
        }

        public async Task GetSaveGame()
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            SaveGame saveGame = _saveGameService.GetActiveSaveGame(username);
            await Clients.Client(Context.ConnectionId).ReceiveSaveGame(saveGame);
        }
    }
}
