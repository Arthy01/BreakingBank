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
        private readonly GameService _gameService;

        // User, connectionID
        private static readonly ConcurrentDictionary<User, string> _connections = new();

        public GameHub(ILogger<GameHub> logger, SessionService sessionService, GameService gameService) 
        {
            _logger = logger;
            _sessionService = sessionService;
            _gameService = gameService;
        }

        public static IReadOnlyDictionary<User, string> GetAllConnections()
        {
            return _connections;
        }

        public override async Task OnConnectedAsync()
        {
            User user = User.GetByClaims(Context.User);

            Session? session = _sessionService.GetUserConnectedSession(user);

            if (session == null)
            {
                _logger.LogInformation($"User {user.Username} ({user.ID}) has not joined a session and therefore cant connect to the GameHub!");
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, session.SaveGame.MetaData.ID);

            _logger.LogInformation($"{user.Username} connected to GameHub and was added to Group {session.SaveGame.MetaData.ID}");
            _connections[user] = Context.ConnectionId;
            
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            User user = User.GetByClaims(Context.User);

            _sessionService.LeaveSession(user, out string msg);

            _logger.LogInformation($"{user.Username} disconnected from GameHub");
            _connections.TryRemove(user, out _);
            
            return base.OnDisconnectedAsync(exception);
        }

        public void Test()
        {
            _logger.LogInformation($"{User.GetByClaims(Context.User).Username} sent a Message!");
        }

        public void Click(GameService.Clickable clickable) 
        {
            User user = User.GetByClaims(Context.User);

            _gameService.OnClickableClicked(user, clickable);
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
