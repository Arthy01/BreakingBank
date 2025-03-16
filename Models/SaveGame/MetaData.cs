namespace BreakingBank.Models.SaveGame
{
    public class MetaData
    {
        public int OwnerUserID { get; }
        public List<int> CoOwnerUserIDs { get; } = new();
        public string Name { get; } = string.Empty;
        public string ID { get; } = string.Empty;

        public MetaData(User owner, string name)
        {
            OwnerUserID = owner.ID;
            Name = name;
            ID = Guid.NewGuid().ToString();
        }
    }
}
