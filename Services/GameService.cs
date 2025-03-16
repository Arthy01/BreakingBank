using BreakingBank.Hubs;
using BreakingBank.Models;
using BreakingBank.Models.SaveGame;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace BreakingBank.Services
{
    public class GameService : BackgroundService
    {
        public event Action? OnTick;

        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly SessionService _sessionService;
        private readonly ILogger<GameService> _logger;
        private readonly GameSettings _gameSettings;

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
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"TickService gestartet! (TickRate: {1000/_gameSettings.TickDelay} Ticks/s)");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                OnTick?.Invoke();

                await Task.Delay(_gameSettings.TickDelay, stoppingToken);
            }
        }

        private void HandleTick()
        {
            HandleProcessingUnits();

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

                    foreach (var x in session.SaveGame.Processing.Printers.Value!.DirtyData)
                    {
                        Console.WriteLine(x.Key + " => " + x.Value);
                    }

                    await _hubContext.Clients.Group(session.SaveGame.MetaData.ID).ReceiveTick(session.SaveGame.DirtyData);
                    session.SaveGame.ClearDirtyData();
                });
            }
        }


        private void HandleProcessingUnits()
        {

        }

        public void OnClickableClicked(User user, Clickable clickable)
        {
            if (!TryGetSession(user, out Session session))
                return;

            switch (clickable) 
            {
                default:
                    return;
                case Clickable.Cartridge:
                    session.SaveGame.Economy.Cartridges.Value += 1;
                    return;
                case Clickable.Paper:
                    session.SaveGame.Economy.Paper.Value += 1;
                    return;
                case Clickable.Printer:
                    session.SaveGame.Processing.Printers.Value!.CurrentClicks.Value += 1;
                    session.SaveGame.Processing.Printers.SetDirty();
                    return;
                case Clickable.WashingMachine:
                    session.SaveGame.Processing.WashingMachines.Value!.CurrentClicks.Value += 1;
                    session.SaveGame.Processing.WashingMachines.SetDirty();
                    return;
                case Clickable.Dryer:
                    session.SaveGame.Processing.Dryers.Value!.CurrentClicks.Value += 1;
                    session.SaveGame.Processing.Dryers.SetDirty();
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
