
namespace BreakingBank.Models
{
    public class Session
    {
        public List<User> Users { get; } = new();
        public SaveGame.SaveGame SaveGame { get; private set; }
        public readonly object SaveGameLock = new();

        public Session(SaveGame.SaveGame saveGame) 
        {
            SaveGame = saveGame;
        }

        public void Join(User user) 
        {
            Users.Add(user);
        }

        public void Leave(User user)
        {
            Users.Remove(user);
        }
    }
}
