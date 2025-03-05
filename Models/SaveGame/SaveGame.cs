using System.Text.Json.Serialization;

namespace BreakingBank.Models.SaveGame
{
    public class SaveGame
    {
        public EconomyData Economy { get; private set; } = new();

        [JsonIgnore]
        public IReadOnlyDictionary<string, object> DirtyData => _dirtyData;

        private readonly Dictionary<string, object> _dirtyData = new();

        public SaveGame()
        {
            Economy.OnDirtyStateChanged += () => OnDirtyStateChanged(Economy, nameof(Economy));
        }

        public void ClearDirtyData()
        {
            Economy.ClearDirtyData();
        }

        private void OnDirtyStateChanged(SaveGameData data, string fieldName)
        {
            if (data.DirtyData.Count > 0)
            {
                _dirtyData[fieldName.ToLower()] = data.DirtyData;
            }
            else
            {
                _dirtyData.Remove(fieldName.ToLower());
            }
        }
    }
}
