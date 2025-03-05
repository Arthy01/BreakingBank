using BreakingBank.Hubs;
using BreakingBank.Models.SaveGame;
using Microsoft.AspNetCore.SignalR;

namespace BreakingBank.Services
{
    public class GameHubTickService : BackgroundService
    {
        private const int TICK_RATE_MS = 100;

        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly ISaveGameService _saveGameService;
        private readonly ILogger<GameHubTickService> _logger;

        public GameHubTickService(IHubContext<GameHub, IGameClient> hubContext, ISaveGameService saveGameService, ILogger<GameHubTickService> logger)
        {
            _hubContext = hubContext;
            _saveGameService = saveGameService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TickService gestartet!");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TICK_RATE_MS, stoppingToken);

                var connections = GameHub.GetAllConnections();
                if (!connections.Any()) continue;

                _logger.LogInformation($"Tick: {connections.Count} verbundene Clients");

                foreach (var (connectionId, username) in connections)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            SaveGame saveGame = _saveGameService.GetActiveSaveGame(username);
                            if (saveGame != null && saveGame.DirtyData.Count > 0)
                            {
                                await _hubContext.Clients.Client(connectionId).ReceiveTick(saveGame.DirtyData);
                                saveGame.ClearDirtyData();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Fehler beim Senden an {username}: {ex}");
                        }
                    }, stoppingToken);
                }
            }
        }
    }
}
