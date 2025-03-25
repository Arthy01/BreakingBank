using BreakingBank.Hubs;
using BreakingBank.Models;
using BreakingBank.Models.SaveGame;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BreakingBank.Services
{
    public class SessionService
    {
        public event Action<Session>? OnSessionClosed;

        private readonly ILogger<SessionService> _logger;
        private readonly ISaveGameService _saveGameService;
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        private List<Session> _activeSessions = [];
        private Dictionary<User, Session> _sessionsByUser = new();

        public SessionService(ILogger<SessionService> logger, ISaveGameService saveGameService, IHubContext<GameHub, IGameClient> hubContext)
        {
            _logger = logger;
            _saveGameService = saveGameService;
            _hubContext = hubContext;
        }

        public IReadOnlyList<Session> GetAllActiveSessions() => _activeSessions;
        public IReadOnlyDictionary<User, Session> GetAllSessionsByUser() => _sessionsByUser;
        public Session? GetSessionByUser(User user) => _sessionsByUser.GetValueOrDefault(user);

        public bool CreateSession(User user, string saveGameID, out string msg)
        {
            msg = $"Session with SaveGameID {saveGameID} successfully created!";

            if (GetUserConnectedSession(user) != null)
            {
                msg = "User already in a session: " + GetUserConnectedSession(user)!.SaveGame.MetaData.ID;
                _logger.LogInformation($"{user.ToString()} tried to create a Session. The action failed because he is already in a session!");
                return false;
            }

            SaveGame? saveGame = _saveGameService.GetSaveGame(saveGameID).Result;

            if (saveGame == null)
            {
                msg = $"SaveGame with id {saveGameID} does not exist.";
                _logger.LogInformation($"{user.ToString()} tried to create a Session. The action failed because the SaveGameID doesnt exist!");
                return false;
            }

            Session session = new Session(saveGame);

            _activeSessions.Add(session);
            _logger.LogInformation($"{user.ToString()} successfully created a session with ID {saveGameID}");

            return true;
        }

        public bool JoinSession(User user, string saveGameID, out string msg)
        {
            msg = $"Joined session with SaveGameID {saveGameID} successfully!";

            if (GetUserConnectedSession(user) != null)
            {
                msg = "User already in a session: " + GetUserConnectedSession(user)!.SaveGame.MetaData.ID;
                _logger.LogInformation(msg);
                return false;
            }

            Session? session = _activeSessions.Find(x => x.SaveGame.MetaData.ID == saveGameID);

            if (session == null)
            {
                msg = $"There is no session with SaveGameID {saveGameID}!";
                _logger.LogInformation(msg);
                return false;
            }

            if (session.Users.Contains(user))
            {
                msg = "The user has joined the session already!";
                _logger.LogInformation(msg);
                return false;
            }

            session.Users.Add(user);
            _sessionsByUser[user] = session;
            _logger.LogInformation(msg);
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
            _sessionsByUser.Remove(user);

            if (user.ID == session.SaveGame.MetaData.OwnerUserID)
            {
                CloseSession(session);
            }

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

            CloseSession(session);

            return true;
        }

        private void CloseSession(Session session)
        {
            _hubContext.Clients.Group(session.SaveGame.MetaData.ID).ForceDisconnect();

            _activeSessions.Remove(session);
            foreach (User sessionUser in session.Users)
            {
                _sessionsByUser.Remove(sessionUser);
            }

            _saveGameService.UpdateSaveGame(session.SaveGame);

            OnSessionClosed?.Invoke(session);
        }

        public Session? GetUserConnectedSession(User user)
        {
            return _activeSessions.Find(x => x.Users.Contains(user));
        }

        public bool SaveSession(User user, out string msg)
        {
            msg = "Session successfully saved!";

            Session? session = GetUserConnectedSession(user);

            if (session == null)
            {
                msg = $"User {user.ToString()} is not currently in a session!";
                return false;
            }

            SaveGame saveGame = session.SaveGame;

            return _saveGameService.UpdateSaveGame(saveGame).Result;
        }
    }
}
