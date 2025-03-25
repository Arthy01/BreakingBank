namespace BreakingBank.Models.SaveGame
{
    public class Investment : SaveGameData
    {
        public string Name { get; }
        public string Description { get; }
        public DirtyField<bool> IsPurchased { get; } = new();
        public ulong Cost { get; }
        public ulong RevenuePerSecond { get; }

        public Investment(string name, string description, bool isPurchased, ulong cost, ulong revenuePerSecond)
        {
            Name = name;
            Description = description;
            IsPurchased = new DirtyField<bool>() { Value = isPurchased };
            Cost = cost;
            RevenuePerSecond = revenuePerSecond;

            IsPurchased.OnDirtyStateChanged += () => HandleDirtyStateChanged(IsPurchased, nameof(IsPurchased));
        }

        public override void ClearDirtyData()
        {
            IsPurchased.ClearDirty();

            base.ClearDirtyData();
        }

        public bool CanBuy()
        {
            return true;
        }

        public void Buy()
        {
            if (!CanBuy()) return;

            //game.Money -= GetCost();
            IsPurchased.Value = true;
        }
    }
}
