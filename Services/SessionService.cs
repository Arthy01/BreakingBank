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

        public IReadOnlyList<Session> GetAllSessions() => _activeSessions;

        public bool CreateSession(User user, string saveGameID, out string msg)
        {
            msg = $"Session with SaveGameID {saveGameID} successfully created!";

            if (GetUserConnectedSession(user) != null)
            {
                msg = "User already in a session: " + GetUserConnectedSession(user)!.SaveGame.MetaData.ID;
                return false;
            }

            SaveGame? saveGame = _saveGameService.GetSaveGame(saveGameID);

            if (saveGame == null)
            {
                msg = $"SaveGame with id {saveGameID} does not exist.";
                return false;
            }

            _activeSessions.Add(new Session(saveGame));
            return true;
        }

        public bool JoinSession(User user, string saveGameID, out string msg)
        {
            msg = $"Joined session with SaveGameID {saveGameID} successfully!";

            if (GetUserConnectedSession(user) != null)
            {
                msg = "User already in a session: " + GetUserConnectedSession(user)!.SaveGame.MetaData.ID;
                return false;
            }

            Session? session = _activeSessions.Find(x => x.SaveGame.MetaData.ID == saveGameID);

            if (session == null)
            {
                msg = $"There is no session with SaveGameID {saveGameID}!";
                return false;
            }

            if (session.Users.Contains(user))
            {
                msg = "The user has joined the session already!";
                return false;
            }

            session.Users.Add(user);
            return true;
        }

        public bool LeaveSession(User user, out string msg)
        {
            msg = "Successfully left the current session!"; 

            Session? session = GetUserConnectedSession(user);

            if (session == null)
            {
                msg = "The user has not entered a session yet, so he cant leave a session!";
                return false;
            }

            bool userFound = GameHub.GetAllConnections().TryGetValue(user, out string? connectionID);
            if (connectionID != null)
                _hubContext.Clients.Client(connectionID).ForceDisconnect();

            session.Users.Remove(user);
            return true;
        }

        public bool CloseSession(User user, out string msg)
        {
            msg = "Successfully closed the session!";

            Session? session = GetUserConnectedSession(user);

            if (session == null)
            {
                msg = "The user has not entered a session yet, so he cant close a session!";
                return false;
            }

            if (user.ID != session.SaveGame.MetaData.OwnerUserID && !session.SaveGame.MetaData.CoOwnerUserIDs.Contains(user.ID))
            {
                msg = "The user is not permitted to close the session!";
                return false;
            }

            _hubContext.Clients.Group(session.SaveGame.MetaData.ID).ForceDisconnect();
            _activeSessions.Remove(session);
            return true;
        }

        public Session? GetUserConnectedSession(User user)
        {
            return _activeSessions.Find(x => x.Users.Contains(user));
        }
    }
}
