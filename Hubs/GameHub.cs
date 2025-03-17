using BreakingBank.Models.SaveGame;
using BreakingBank.Models;
using BreakingBank.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using BreakingBank.Services.Game;

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

        public void RandomClicks(int clickAmount = 0)
        {
            User user = User.GetByClaims(Context.User);

            Random rnd = new Random();

            if (clickAmount <= 0)
                clickAmount = rnd.Next(1, 100);

            _logger.LogInformation($"Clicking random {clickAmount} times!");

            for (int i = 0; i < clickAmount; i++)
            {
                Click((GameService.Clickable)rnd.Next(1, 6));
            }
        }

        public void Upgrade()
        {
            User user = User.GetByClaims(Context.User);

            Session? session = _sessionService.GetSessionByUser(user);

            if (session == null)
                return;

            session.SaveGame.Upgrades.Upgrades[0].Value!.Buy();
        }

        public async void GetSaveGame()
        {
            User user = User.GetByClaims(Context.User);

            Session? session = _sessionService?.GetSessionByUser(user);

            if (session == null)
                return;

            await Clients.Group(session.SaveGame.MetaData.ID).ReceiveSaveGame(session.SaveGame);
        }
    }
}
