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
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            try
            {
                using JsonDocument doc = JsonDocument.Parse(data);
                
                MetaData? metaData = DeserializeMetaData(doc.RootElement);

                if (metaData == null)
                    throw new Exception("Deserialization of MetaData has failed!");

                EconomyData? economyData = DeserializeEconomyData(doc.RootElement);

                if (economyData == null)
                    throw new Exception("Deserialization of EconomyData has failed!");

                ProcessingData? processingData = DeserializeProcessingData(doc.RootElement);

                if (processingData == null)
                    throw new Exception("Deserialization of ProcessingData has failed!");

                UpgradeData? upgradeData = DeserializeUpgradeData(doc.RootElement);

                if (upgradeData == null)
                    throw new Exception("Deserialization of UpgradeData has failed!");

                saveGame = new SaveGame(metaData, economyData, processingData, upgradeData);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deserialization failed: " + ex.Message + ex.StackTrace);
                saveGame = null!;
                return false;
            }
        }

        private static MetaData? DeserializeMetaData(JsonElement root)
        {
            if (!root.TryGetProperty("metaData", out JsonElement metaDataElement))
                return null;

            if (!metaDataElement.TryGetProperty("id", out JsonElement id))
                return null;

            if (!metaDataElement.TryGetProperty("name", out JsonElement name))
                return null;

            if (!metaDataElement.TryGetProperty("ownerUserID", out JsonElement ownerUserID))
                return null;

            if (!metaDataElement.TryGetProperty("coOwnerUserIDs", out JsonElement coUserOwnerIDs))
                return null;

            List<int> coUserIDs = new();
            foreach (JsonElement idElement in coUserOwnerIDs.EnumerateArray())
            {
                if (idElement.TryGetInt32(out int coID))
                {
                    coUserIDs.Add(coID);
                }
            }

            return new MetaData(id.GetString(), name.GetString(), ownerUserID.GetInt32(), coUserIDs);
        }

        private static EconomyData? DeserializeEconomyData(JsonElement root)
        {
            if (!root.TryGetProperty("economy", out JsonElement economyDataElement))
                return null;

            if (!economyDataElement.TryGetProperty("paper", out JsonElement paper))
                return null;

            if (!economyDataElement.TryGetProperty("wetMoney", out JsonElement wetMoney))
                return null;

            if (!economyDataElement.TryGetProperty("cartridges", out JsonElement cartridges))
                return null;

            if (!economyDataElement.TryGetProperty("cleanMoney", out JsonElement cleanMoney))
                return null;

            if (!economyDataElement.TryGetProperty("dirtyMoney", out JsonElement dirtyMoney))
                return null;

            return new EconomyData(
                cleanMoney.GetUInt64(),
                wetMoney.GetUInt64(),
                dirtyMoney.GetUInt64(),
                cartridges.GetUInt64(),
                paper.GetUInt64()
                );
        }

        private static ProcessingData? DeserializeProcessingData(JsonElement root)
        {
            if (!root.TryGetProperty("processing", out JsonElement processingDataElement))
                return null;

            if (!processingDataElement.TryGetProperty("dryers", out JsonElement dryersDataElement))
                return null;

            if (!processingDataElement.TryGetProperty("printers", out JsonElement printersDataElement))
                return null;

            if (!processingDataElement.TryGetProperty("washingMachines", out JsonElement washingMachinesDataElement))
                return null;

            ProcessingUnit? printerUnit = DeserializeProcessingUnit(printersDataElement);
            if (printerUnit == null) 
                return null;

            ProcessingUnit? washingUnit = DeserializeProcessingUnit(washingMachinesDataElement);
            if (washingUnit == null) 
                return null;
            
            ProcessingUnit? dryerUnit = DeserializeProcessingUnit(dryersDataElement);
            if (dryerUnit == null)
                return null;

            return new ProcessingData(new() { Value = printerUnit }, new() { Value = washingUnit }, new() { Value = dryerUnit });
        }

        private static ProcessingUnit? DeserializeProcessingUnit(JsonElement unitElement)
        {
            if (!unitElement.TryGetProperty("count", out JsonElement countElement))
                return null;

            if (!unitElement.TryGetProperty("maxCapacity", out JsonElement maxCapacityElement))
                return null;

            if (!unitElement.TryGetProperty("usedCapacity", out JsonElement usedCapacityElement))
                return null;

            if (!unitElement.TryGetProperty("currentClicks", out JsonElement currentClicksElement))
                return null;

            if (!unitElement.TryGetProperty("requiredClicks", out JsonElement requiredClicksElement))
                return null;

            return new ProcessingUnit(
                countElement.GetUInt64(), 
                usedCapacityElement.GetUInt64(), 
                maxCapacityElement.GetUInt64(), 
                currentClicksElement.GetUInt64(), 
                requiredClicksElement.GetUInt64()
                );
        }

        private static UpgradeData? DeserializeUpgradeData(JsonElement root)
        {
            if (!root.TryGetProperty("upgrades", out JsonElement upgradesDataElement))
                return null;

            if (!upgradesDataElement.TryGetProperty("upgrades", out JsonElement upgradesElement))
                return null;

            List<DirtyField<Upgrade>> upgradesList = new();
            foreach (JsonElement upgradeElement in upgradesElement.EnumerateArray())
            {
                if (!upgradeElement.TryGetProperty("name", out JsonElement nameElement))
                    continue;

                if (!upgradeElement.TryGetProperty("level", out JsonElement levelElement))
                    continue;

                if (!upgradeElement.TryGetProperty("baseCost", out JsonElement baseCostElement))
                    continue;

                if (!upgradeElement.TryGetProperty("baseEffect", out JsonElement baseEffectElement))
                    continue;

                if (!upgradeElement.TryGetProperty("description", out JsonElement descriptionElement))
                    continue;

                if (!upgradeElement.TryGetProperty("costIncrease", out JsonElement costIncreaseElement))
                    continue;

                upgradesList.Add(new DirtyField<Upgrade>()
                {
                    Value = new Upgrade(
                    nameElement.GetString(),
                    descriptionElement.GetString(),
                    levelElement.GetUInt64(),
                    baseCostElement.GetUInt64(),
                    costIncreaseElement.GetUInt64(),
                    baseEffectElement.GetDouble()
                    )
                });
            }

            return new UpgradeData(upgradesList);
        }


        private SaveGame(MetaData metaData)
        {
            MetaData = metaData;

            Initialize();
        }

        public SaveGame(MetaData metaData, EconomyData economyData, ProcessingData processingData, UpgradeData upgradeData)
        {
            MetaData = metaData;
            Economy = economyData;
            Processing = processingData;
            Upgrades = upgradeData;

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
