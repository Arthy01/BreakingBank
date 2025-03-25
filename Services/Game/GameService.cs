using BreakingBank.Hubs;
using BreakingBank.Models;
using BreakingBank.Models.SaveGame;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace BreakingBank.Services.Game
{
    public class GameService : BackgroundService
    {
        public event Action? OnTick;

        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly SessionService _sessionService;
        private readonly ILogger<GameService> _logger;
        private readonly GameSettings _gameSettings;

        private readonly AutoClicker _autoClicker;

        public enum Clickable
        {
            Undefined = 0,
            Cartridge = 1,
            Paper = 2,
            Printer = 3,
            WashingMachine = 4,
            Dryer = 5
        }

        public GameService(IHubContext<GameHub, IGameClient> hubContext, SessionService sessionService, IOptions<GameSettings> gameSettings, ILogger<GameService> logger)
        {
            _hubContext = hubContext;
            _sessionService = sessionService;
            _gameSettings = gameSettings.Value;
            _logger = logger;

            OnTick += HandleTick;

            _autoClicker = new(_sessionService, this, _gameSettings, logger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"TickService gestartet! (TickRate: {_gameSettings.TickRate} Ticks/s) (Tick Delay: {_gameSettings.TickDelay} ms)");

            while (!stoppingToken.IsCancellationRequested)
            {
                OnTick?.Invoke();

                await Task.Delay(_gameSettings.TickDelay, stoppingToken);
            }
        }


        private void HandleTick()
        {
            HandleAutoClicker();

            SendDirtyDataToAllClients();
        }

        private void SendDirtyDataToAllClients()
        {
            IReadOnlyList<Session> sessions = _sessionService.GetAllActiveSessions();

            foreach (Session session in sessions)
            {
                Task.Run(async () =>
                {
                    if (session.SaveGame.DirtyData.Count == 0)
                        return;

                    await _hubContext.Clients.Group(session.SaveGame.MetaData.ID).ReceiveTick(session.SaveGame.DirtyData);
                    session.SaveGame.ClearDirtyData();
                });
            }
        }

        private void HandleAutoClicker()
        {
            _autoClicker.HandleAutoClicker();
        }

        public void OnClickableClicked(User user, Clickable clickable)
        {
            if (!TryGetSession(user, out Session session))
                return;

            ulong amount = (ulong)session.SaveGame.Upgrades.Upgrades.Find(x => x.Value!.ID == Upgrade.UpgradeID.Player_Efficiency)!.Value!.GetEffectInt();

            OnClickableClicked(session, clickable, amount);
        }

        public void OnClickableClicked(Session session, Clickable clickable, ulong amount)
        {
            if (amount == 0)
                return;

            switch (clickable)
            {
                default:
                    return;
                case Clickable.Cartridge:
                    session.SaveGame.Economy.Cartridges.Value += amount;
                    return;
                case Clickable.Paper:
                    session.SaveGame.Economy.Paper.Value += amount;
                    return;
                case Clickable.Printer:
                    session.SaveGame.Processing.Printers.Value!.HandleClick(amount);
                    return;
                case Clickable.WashingMachine:
                    session.SaveGame.Processing.WashingMachines.Value!.HandleClick(amount);
                    return;
                case Clickable.Dryer:
                    session.SaveGame.Processing.Dryers.Value!.HandleClick(amount);
                    return;
            }
        }


        private bool TryGetSession(User user, out Session session)
        {
            session = _sessionService.GetSessionByUser(user)!;

            if (session == null)
                return false;

            return true;
        }
    }
}
