using System.Text.Json.Serialization;
using BreakingBank.Helpers;
namespace BreakingBank.Models.SaveGame
{
    public class SaveGame
    {
        public MetaData MetaData { get; private set; }

        public EconomyData Economy { get; private set; } = new();
        public ProcessingData Processing { get; private set; } = new();
        public UpgradeData Upgrades { get; private set; } = new();

        [JsonIgnore]
        public IReadOnlyDictionary<string, object> DirtyData => _dirtyData;

        private readonly Dictionary<string, object> _dirtyData = new();

        public static SaveGame Create(User owner, string name)
        {
            MetaData meta = new MetaData(owner, name);

            return new SaveGame(meta);
        }

        public static SaveGame Load(string saveGameID)
        {
            return null;
        }

        private SaveGame(MetaData metaData)
        {
            MetaData = metaData;

            Initialize();
        }

        private void Initialize()
        {
            Economy.OnDirtyStateChanged += () => OnDirtyStateChanged(Economy, nameof(Economy));
            Processing.OnDirtyStateChanged += () => OnDirtyStateChanged(Processing, nameof(Processing));
            Upgrades.OnDirtyStateChanged += () => OnDirtyStateChanged(Upgrades, nameof(Upgrades));

            ClearDirtyData();
        }

        public void ClearDirtyData()
        {
            Economy.ClearDirtyData();
            Processing.ClearDirtyData();
            Upgrades.ClearDirtyData();
        }

        private void OnDirtyStateChanged(SaveGameData data, string fieldName)
        {
            if (data.DirtyData.Count > 0)
            {
                _dirtyData[fieldName.ToCamelCase()] = data.DirtyData;
                Console.WriteLine("DIRTY STATE CHANGED (SET DIRTY): " + fieldName);
            }
            else
            {
                _dirtyData.Remove(fieldName.ToCamelCase());
                Console.WriteLine("DIRTY STATE CHANGED (CLEAR DIRTY): " + fieldName);
            }
        }
    }
}
