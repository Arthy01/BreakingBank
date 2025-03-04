
namespace BreakingBank.Models.SaveGame
{
    public class EconomyData : SaveGameData
    {
        public DirtyField<float> Money { get; } = new();
        public DirtyField<float> PassiveIncome { get; } = new();

        public EconomyData()
        {
            Money.OnDirtyStateChanged += () => HandleDirtyStateChanged(Money, nameof(Money));
            PassiveIncome.OnDirtyStateChanged += () => HandleDirtyStateChanged(PassiveIncome, nameof(PassiveIncome));

            Money.SetDirty();
            PassiveIncome.SetDirty();
        }

        public override void ClearDirtyData()
        {
            Money.ClearDirty();
            PassiveIncome.ClearDirty();

            base.ClearDirtyData();
        }
    }
}
