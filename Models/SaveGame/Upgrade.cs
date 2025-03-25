namespace BreakingBank.Models.SaveGame
{
    public class Upgrade : SaveGameData
    {
        public UpgradeID ID { get; }
        public string Name { get; }
        public string Description { get; }
        public DirtyField<ulong> Level { get; } = new();
        public ulong BaseCost { get; }
        public ulong CostIncrease { get; }
        public double BaseEffect { get; }
        public double EffectIncrease { get; }

        public enum UpgradeID
        {
            Undefined,

            EmployeeCount_Paper,
            EmployeeCount_Cartridge,
            EmployeeCount_Printer,
            EmployeeCount_WashingMachine,
            EmployeeCount_Dryer,

            EmployeeSpeed_Paper,
            EmployeeSpeed_Cartridge,
            EmployeeSpeed_Printer,
            EmployeeSpeed_WashingMachine,
            EmployeeSpeed_Dryer,

            EmployeeEfficiency_Paper,
            EmployeeEfficiency_Cartridge,
            EmployeeEfficiency_Printer,
            EmployeeEfficiency_WashingMachine,
            EmployeeEfficiency_Dryer,

            Player_Efficiency
        }

        public Upgrade(UpgradeID id, string name, string description, ulong level, ulong baseCost, ulong costIncrease, double baseEffect, double effectIncrease)
        {
            ID = id;
            Name = name;
            Description = description;
            Level = new DirtyField<ulong>() { Value = level };
            BaseCost = baseCost;
            CostIncrease = costIncrease;
            BaseEffect = baseEffect;
            EffectIncrease = effectIncrease;

            Level.OnDirtyStateChanged += () => HandleDirtyStateChanged(Level, nameof(Level));
        }

        public override void ClearDirtyData()
        {
            Level.ClearDirty();

            base.ClearDirtyData();
        }

        public ulong GetCost()
        {
            return BaseCost + (ulong)(Level.Value * CostIncrease * (1 + 0.01 * Level.Value));
        }

        public int GetEffectInt()
        {
            // Every 10 Levels, 10% Bonus
            return (int)(BaseEffect + ((Level.Value * EffectIncrease * (1 + (Level.Value / 10) * 0.5))));
        }

        public double GetEffectDouble()
        {
            // Every 10 Levels, 10% Bonus
            return BaseEffect + (Level.Value * EffectIncrease * (1 + (Level.Value / 10) * 0.5));
        }

        public bool CanBuy()
        {
            return true;
        }

        public void Buy()
        {
            if (!CanBuy()) return;

            //game.Money -= GetCost();
            Level.Value++;
        }
    }
}
