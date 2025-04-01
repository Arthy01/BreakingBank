namespace BreakingBank.Models.SaveGame
{
    public class Investment : SaveGameData
    {
        public InvestmentID ID { get; }
        public string Name { get; }
        public string Description { get; }
        public DirtyField<bool> IsPurchased { get; } = new();
        public ulong Cost { get; }
        public ulong RevenuePerSecond { get; }

        private EconomyData? _economyData;

        public enum InvestmentID
        {
            Undefined,

            Laundromat,
            Pizzeria,
            SelfStorage,
            Gym,
            FranchiseRestaurant,
            LuxuryHotel,
            SoccerClub,
            PrivateBank,
            StreamingPlatform,
            TelecommunicationsNetwork,
            ChipFactory,
            SpaceCompany,
            MultinationalHolding,
            BuyTheWorld
        }

        public Investment(InvestmentID id, string name, string description, bool isPurchased, ulong cost, ulong revenuePerSecond)
        {
            ID = id;
            Name = name;
            Description = description;
            IsPurchased = new DirtyField<bool>() { Value = isPurchased };
            Cost = cost;
            RevenuePerSecond = revenuePerSecond;

            IsPurchased.OnDirtyStateChanged += () => HandleDirtyStateChanged(IsPurchased, nameof(IsPurchased));
        }

        public void SetEconomyData(EconomyData economyData)
        {
            _economyData = economyData;
        }

        public override void ClearDirtyData()
        {
            IsPurchased.ClearDirty();

            base.ClearDirtyData();
        }

        public bool CanBuy()
        {
            return !IsPurchased.Value && _economyData!.CleanMoney.Value >= Cost;
        }

        public void Buy()
        {
            if (!CanBuy()) return;

            _economyData!.CleanMoney.Value -= Cost;
            IsPurchased.Value = true;
        }
    }
}
