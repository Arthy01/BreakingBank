using BreakingBank.Models.SaveGame;
using BreakingBank.Models;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace BreakingBank.Hubs
{
    [Authorize]
    public class GameHub : Hub<IGameClient>
    {
        private readonly ILogger<GameHub> _logger;
        private readonly SessionService _sessionService;

        // ConnectionID, Username
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        public GameHub(ILogger<GameHub> logger, SessionService sessionService) 
        {
            _logger = logger;
            _sessionService = sessionService;
        }

        public static string? GetUsernameFromConnectionId(string connectionId)
        {
            return _connections.TryGetValue(connectionId, out var username) ? username : null;
        }

        public static IReadOnlyDictionary<string, string> GetAllConnections()
        {
            return _connections;
        }

        public override async Task OnConnectedAsync()
        {
            string? username = Context.User?.Identity?.Name ?? null;
            int? userID = null;

            if (int.TryParse(Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int result))
            {
                userID = result;
            }

            if (userID == null || username == null)
            {
                Context.Abort();
                return;
            }

            Session? session = _sessionService.GetUserConnectedSession(new User(userID.Value, username));

            if (session == null)
            {
                Context.Abort();
                return;
            }

            _logger.LogInformation($"{username} connected to GameHub");
            _connections[Context.ConnectionId] = username;

            await Groups.AddToGroupAsync(Context.ConnectionId, session.SaveGame.MetaData.ID);
            
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string username = Context.User?.Identity?.Name ?? "Unknown User";
            string? userID = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;

            _logger.LogInformation($"{username} disconnected from GameHub");
            _connections.TryRemove(Context.ConnectionId, out _);
            
            return base.OnDisconnectedAsync(exception);
        }

        /*
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
        */
    }
}
