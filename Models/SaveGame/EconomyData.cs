
using System.Text.Json.Serialization;

namespace BreakingBank.Models.SaveGame
{
    public class EconomyData : SaveGameData
    {
        public DirtyField<ulong> CleanMoney { get; } = new();
        public DirtyField<ulong> WetMoney { get; } = new();
        public DirtyField<ulong> DirtyMoney { get; } = new();
        public DirtyField<ulong> Cartridges { get; } = new();
        public DirtyField<ulong> Paper { get; } = new();

        public EconomyData()
        {
            RegisterEvents();
        }

        public EconomyData(long cleanMoney, long wetMoney, long dirtyMoney, long cartridges, long paper)
        {
            CleanMoney = new() { Value = Convert.ToUInt64(cleanMoney) };
            WetMoney = new() { Value = Convert.ToUInt64(wetMoney) };
            DirtyMoney = new() { Value = Convert.ToUInt64(dirtyMoney) };
            Cartridges = new() { Value = Convert.ToUInt64(cartridges) };
            Paper = new() { Value = Convert.ToUInt64(paper) };

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            CleanMoney.OnDirtyStateChanged += () => HandleDirtyStateChanged(CleanMoney, nameof(CleanMoney));
            WetMoney.OnDirtyStateChanged += () => HandleDirtyStateChanged(WetMoney, nameof(WetMoney));
            DirtyMoney.OnDirtyStateChanged += () => HandleDirtyStateChanged(DirtyMoney, nameof(DirtyMoney));

            Cartridges.OnDirtyStateChanged += () => HandleDirtyStateChanged(Cartridges, nameof(Cartridges));
            Paper.OnDirtyStateChanged += () => HandleDirtyStateChanged(Paper, nameof(Paper));
        }

        public override void ClearDirtyData()
        {
            CleanMoney.ClearDirty();
            WetMoney.ClearDirty();
            DirtyMoney.ClearDirty();

            Cartridges.ClearDirty();
            Paper.ClearDirty();

            base.ClearDirtyData();
        }
    }
}
