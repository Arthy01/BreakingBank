using BreakingBank.Hubs;
using BreakingBank.Models;
using BreakingBank.Models.SaveGame;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BreakingBank.Services
{
    public class SessionService
    {
        private readonly ILogger<SessionService> _logger;
        private readonly ISaveGameService _saveGameService;
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        private List<Session> _activeSessions = [];

        public SessionService(ILogger<SessionService> logger, ISaveGameService saveGameService, IHubContext<GameHub, IGameClient> hubContext)
        {
            _logger = logger;
            _saveGameService = saveGameService;
            _hubContext = hubContext;
        }

        public bool CreateSession(User user, string saveGameID)
        {
            SaveGame? saveGame = _saveGameService.GetSaveGame(saveGameID);

            if (saveGame == null)
                return false;

            _activeSessions.Add(new Session(saveGame));
            return true;
        }

        public bool JoinSession(User user, string saveGameID)
        {
            Session? session = _activeSessions.Find(x => x.SaveGame.MetaData.ID == saveGameID);

            if (session == null)
                return false;

            if (session.Users.Contains(user))
                return false;

            session.Users.Add(user);
            return true;
        }

        public bool LeaveSession(User user)
        {
            Session? session = _activeSessions.Find(x => x.Users.Contains(user));

            if (session == null) 
                return false;

            session.Users.Remove(user);
            return true;
        }

        public void CloseSession(User user, string saveGameID)
        {

        }

        public Session? GetUserConnectedSession(User user)
        {
            return _activeSessions.Find(x => x.Users.Contains(user));
        }
    }
}
