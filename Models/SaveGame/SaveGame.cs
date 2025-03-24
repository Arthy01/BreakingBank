using System.Text.Json;
using System.Text.Json.Serialization;
using BreakingBank.Helpers;
using BreakingBank.JsonConverters;
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

        public static bool Parse(string data, out SaveGame saveGame)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                using var doc = JsonDocument.Parse(data);
                
                if (!doc.RootElement.TryGetProperty("metaData", out JsonElement metaDataElement))
                {
                    saveGame = null!;
                    return false;
                }

                // MetaData-Teil als JSON extrahieren
                var metaDataJson = metaDataElement.GetRawText();
                Console.WriteLine(metaDataJson);
                // In echtes Objekt umwandeln
                var metaData = JsonSerializer.Deserialize<MetaData>(metaDataJson, options);
                if (metaData == null)
                {
                    saveGame = null!;
                    return false;
                }

                Console.WriteLine("name: " + metaData.Name);

                if (!doc.RootElement.TryGetProperty("economy", out JsonElement eco))
                {
                    saveGame = null!;
                    return false;
                }

                var economyDataJson = eco.GetRawText();
                Console.WriteLine(economyDataJson);
                EconomyData e = new EconomyData
                    (
                    eco.GetProperty("cleanMoney").GetInt64(),
                    eco.GetProperty("wetMoney").GetInt64(),
                    eco.GetProperty("dirtyMoney").GetInt64(),
                    eco.GetProperty("cartridges").GetInt64(),
                    eco.GetProperty("paper").GetInt64()
                    );
                Console.WriteLine(e);
                Console.WriteLine(e.Paper.Value);

                saveGame = new SaveGame(metaData);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deserialization failed: " + ex.Message + ex.StackTrace);
                saveGame = null!;
                return false;
            }
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
