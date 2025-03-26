using System.Text.Json.Serialization;

namespace BreakingBank.Models.SaveGame
{
    public class ProcessingUnit : SaveGameData
    {
        [JsonIgnore] public UnitType Type { get; private set; }

        [JsonInclude] public ulong CapacityPerCount { get; private set; }
        [JsonInclude] public ulong RequiredClicks { get; private set; }

        public DirtyField<ulong> Count { get; } = new() { Value = 1 };
        public DirtyField<ulong> UsedCapacity { get; } = new();
        public DirtyField<ulong> CurrentClicks { get; } = new();

        private Dictionary<EconomyData.Resource, ulong> _requiredResourceAmount = new(); // Amounts of resources needed to start the clicking process
        private EconomyData.Resource _resourceOnFinished;

        private EconomyData _economyData;

        public enum UnitType
        {
            Printer,
            WashingMachine,
            Dryer
        }

        public ProcessingUnit(UnitType type, EconomyData economyData)
        {
            Type = type;
            _economyData = economyData;

            Configure();

            RegisterEvents();
        }
        
        public ProcessingUnit(UnitType type, EconomyData economyData, ulong count, ulong usedCapacity, ulong currentClicks)
        {
            Type = type;
            _economyData = economyData;

            Count = new() { Value = count };
            UsedCapacity = new() { Value = usedCapacity };
            CurrentClicks = new() { Value= currentClicks };

            Configure();

            RegisterEvents();
        }

        public void HandleClick(ulong clicks)
        {
            if (CurrentClicks.Value > 0)
                HandleProcessContinueClick(clicks);
            else
                HandleProcessStartClick(clicks);
        }

        private void HandleProcessStartClick(ulong amount)
        {
            if (Type == UnitType.Printer)
            {
                ulong requiredPaperPerPrinter = _requiredResourceAmount[EconomyData.Resource.Paper];
                ulong requiredCartridgesPerPrinter = _requiredResourceAmount[EconomyData.Resource.Cartridges];

                ulong maxPaperPossible = requiredPaperPerPrinter * Count.Value;
                ulong maxCartridgesPossible = requiredCartridgesPerPrinter * Count.Value;

                ulong currentPaperInStock = _economyData.Paper.Value;
                ulong currentCartridgesInStock = _economyData.Cartridges.Value;

                ulong maxPrintersByPaperLoadable = Math.Min(currentPaperInStock / requiredPaperPerPrinter, Count.Value);
                ulong maxPrintersByCartridgesLoadable = Math.Min(currentCartridgesInStock / requiredCartridgesPerPrinter, Count.Value);

                ulong printersToLoad = Math.Min(maxPrintersByPaperLoadable, maxPrintersByCartridgesLoadable);

                if (printersToLoad == 0)
                    return;

                _economyData.RemoveResource(EconomyData.Resource.Paper, printersToLoad * requiredPaperPerPrinter);
                _economyData.RemoveResource(EconomyData.Resource.Cartridges, printersToLoad * requiredCartridgesPerPrinter);

                UsedCapacity.Value = printersToLoad * requiredPaperPerPrinter;
            }
            else
            {
                ulong usedCapacity = Math.Min(CapacityPerCount * Count.Value, _economyData.GetResourceAmount(_requiredResourceAmount.Keys.First()));

                if (usedCapacity == 0)
                    return;

                _economyData.RemoveResource(_requiredResourceAmount.Keys.First(), usedCapacity);

                UsedCapacity.Value = usedCapacity;
            }

            CurrentClicks.Value += amount;

            if (CurrentClicks.Value >= RequiredClicks)
                HandleFinishProcess();
        }

        private void HandleProcessContinueClick(ulong amount)
        {
            CurrentClicks.Value += amount;

            if (CurrentClicks.Value >= RequiredClicks)
                HandleFinishProcess();
        }

        private void HandleFinishProcess()
        {
            _economyData.AddResource(_resourceOnFinished, UsedCapacity.Value);
            ulong leftOverClicks = CurrentClicks.Value - RequiredClicks;

            CurrentClicks.Value = 0;
            UsedCapacity.Value = 0;

            Console.WriteLine("-------- LEFTOVER CLICKS: " + leftOverClicks + " --------");

            if (leftOverClicks > 0)
                HandleClick(leftOverClicks);
        }

        private void Configure()
        {
            if (Type == UnitType.Printer)
            {
                CapacityPerCount = 0;
                RequiredClicks = 15;

                _requiredResourceAmount.Add(EconomyData.Resource.Paper, 10);
                _requiredResourceAmount.Add(EconomyData.Resource.Cartridges, 5);

                _resourceOnFinished = EconomyData.Resource.DirtyMoney;
            }
            else if (Type == UnitType.WashingMachine)
            {
                CapacityPerCount = 25;
                RequiredClicks = 50;

                _requiredResourceAmount.Add(EconomyData.Resource.DirtyMoney, 1);

                _resourceOnFinished = EconomyData.Resource.WetMoney;
            }
            else if (Type == UnitType.Dryer)
            {
                CapacityPerCount = 15;
                RequiredClicks = 30;

                _requiredResourceAmount.Add(EconomyData.Resource.WetMoney, 1);

                _resourceOnFinished = EconomyData.Resource.CleanMoney;
            }
        }

        private void RegisterEvents()
        {
            Count.OnDirtyStateChanged += () => HandleDirtyStateChanged(Count, nameof(Count));
            UsedCapacity.OnDirtyStateChanged += () => HandleDirtyStateChanged(UsedCapacity, nameof(UsedCapacity));
            CurrentClicks.OnDirtyStateChanged += () => HandleDirtyStateChanged(CurrentClicks, nameof(CurrentClicks));
        }

        public override void ClearDirtyData()
        {
            Count.ClearDirty();
            UsedCapacity.ClearDirty();
            CurrentClicks.ClearDirty();

            base.ClearDirtyData();
        }
    }
}
