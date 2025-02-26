namespace BreakingBank.Hubs
{
    public interface ITestingClient
    {
        Task SendMessage(string message);
        Task ReceiveMessage(string message);
        Task UpdateClicks(int totalClicks);
    }
}
