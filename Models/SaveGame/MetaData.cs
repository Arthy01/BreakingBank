using System.Text.Json.Serialization;

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

        [JsonConstructor]
        public MetaData(string id, string name, int ownerUserID, List<int> coOwnerUserIDs)
        {
            ID = id;
            Name = name;
            OwnerUserID = ownerUserID;
            CoOwnerUserIDs = coOwnerUserIDs;
        }
    }
}
