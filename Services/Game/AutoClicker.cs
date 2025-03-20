using BreakingBank.Models;
using BreakingBank.Models.SaveGame;

namespace BreakingBank.Services.Game
{
    public class AutoClicker
    {
        private readonly SessionService _sessionService;
        private readonly GameService _gameService;
        private readonly GameSettings _gameSettings;

        private Cache _progressCache = new();

        private class Cache
        {
            private Dictionary<Session, Dictionary<GameService.Clickable, double>>  _progress = new();

            public void AddSession(Session session)
            {
                _progress[session] = new()
                {
                    { GameService.Clickable.Cartridge, 0 },
                    { GameService.Clickable.Paper, 0 },
                    { GameService.Clickable.Printer, 0 },
                    { GameService.Clickable.WashingMachine, 0 },
                    { GameService.Clickable.Dryer, 0 }
                };
            }

            public void RemoveSession(Session session)
            {
                _progress.Remove(session);
            }

            public ulong AddProgress(Session session, GameService.Clickable clickable, double amount)
            {
                if (!_progress.ContainsKey(session))
                    AddSession(session);

                _progress[session][clickable] += amount;

                ulong clicksToPerform = (ulong)Math.Max(0, _progress[session][clickable]);

                _progress[session][clickable] -= clicksToPerform;
                return clicksToPerform;
            }
        }

        public AutoClicker(SessionService sessionService, GameService gameService, GameSettings gameSettings)
        {
            _sessionService = sessionService;
            _gameService = gameService;
            _gameSettings = gameSettings;

            _sessionService.OnSessionClosed += _progressCache.RemoveSession;
        }

        public void HandleAutoClicker()
        {
            foreach (Session session in _sessionService.GetAllActiveSessions())
            {
                foreach (GameService.Clickable clickable in Enum.GetValues(typeof(GameService.Clickable)))
                {
                    if (clickable == GameService.Clickable.Undefined)
                        continue;

                    double clicksPerSecond = 0.25; // Ausgedacht, wird duch upgrades bestimmt

                    double clicksThisTick = clicksPerSecond / _gameSettings.TickRate;

                    ulong clicksToPerform = _progressCache.AddProgress(session, clickable, clicksThisTick);
                    _gameService.OnClickableClicked(session, clickable, clicksToPerform);
                }
            }
        }
    }
}
