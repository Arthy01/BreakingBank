namespace BreakingBank.Models.SaveGame
{
    public class Upgrade : SaveGameData
    {
        public string Name { get; }
        public string Description { get; }
        public DirtyField<ulong> Level { get; } = new();
        public ulong BaseCost { get; }
        public ulong CostIncrease { get; }
        public double BaseEffect { get; }

        public Upgrade(string name, string description, ulong level, ulong baseCost, ulong costIncrease, double baseEffect)
        {
            Name = name;
            Description = description;
            Level = new DirtyField<ulong>() { Value = level };
            BaseCost = baseCost;
            CostIncrease = costIncrease;
            BaseEffect = baseEffect;
            
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

        public int GetEffect()
        {
            // Every 10 Levels, 10% Bonus
            return (int)(Level.Value * BaseEffect * (1 + (Level.Value / 10) * 0.5));
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
