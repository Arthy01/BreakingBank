
namespace BreakingBank.Models.SaveGame
{
    public class EconomyData : SaveGameData
    {
        public DirtyField<float> Money { get; } = new();
        public DirtyField<float> PassiveIncome { get; } = new();
        public DirtyField<int> Paper { get; } = new();

        public EconomyData()
        {
            Money.OnDirtyStateChanged += () => HandleDirtyStateChanged(Money, nameof(Money));
            PassiveIncome.OnDirtyStateChanged += () => HandleDirtyStateChanged(PassiveIncome, nameof(PassiveIncome));
            Paper.OnDirtyStateChanged += () => HandleDirtyStateChanged(Paper, nameof(Paper));

            Money.SetDirty();
            PassiveIncome.SetDirty();
            Paper.SetDirty();
        }

        public override void ClearDirtyData()
        {
            Money.ClearDirty();
            PassiveIncome.ClearDirty();
            Paper.ClearDirty();

            base.ClearDirtyData();
        }
    }
}
