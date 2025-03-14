using BreakingBank.Hubs;
using BreakingBank.Models;
using BreakingBank.Models.SaveGame;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace BreakingBank.Services
{
    public class GameHubTickService : BackgroundService
    {

        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly ISaveGameService _saveGameService;
        private readonly ILogger<GameHubTickService> _logger;
        private readonly GameSettings _gameSettings;

        public GameHubTickService(IHubContext<GameHub, IGameClient> hubContext, ISaveGameService saveGameService, IOptions<GameSettings> gameSettings, ILogger<GameHubTickService> logger)
        {
            _hubContext = hubContext;
            _saveGameService = saveGameService;
            _gameSettings = gameSettings.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TickService gestartet!");
            /*
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_gameSettings.TickDelay, stoppingToken);

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
            */
        }
    }
}
