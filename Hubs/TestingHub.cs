using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BreakingBank.Hubs
{
    [Authorize]
    public class TestingHub : Hub<ITestingClient>
    {
        private static int _totalClicks = 0;
        private readonly ILogger<TestingHub> _logger;

        public TestingHub(ILogger<TestingHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(string message)
        {
            var username = Context.User?.Identity?.Name ?? "Unknown User";
            await Clients.All.ReceiveMessage(username + ": " + message);
        }

        public async Task WashingmachineClick()
        {
            _logger.LogInformation("Washingmachine clicked!");
            _totalClicks++;
            await Clients.All.UpdateClicks(_totalClicks);
        }

        public async Task ReceiveMessage(string message)
        {
            var username = Context.User?.Identity?.Name ?? "Unknown User";
            _logger.LogInformation("[RECEIVED MESSAGE] " + username + ": " + message);
            await SendMessage("[ECHO] " + message);
        }
    }
}
