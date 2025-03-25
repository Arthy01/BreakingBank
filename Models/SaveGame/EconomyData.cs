
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

        public EconomyData(ulong cleanMoney, ulong wetMoney, ulong dirtyMoney, ulong cartridges, ulong paper)
        {
            CleanMoney = new() { Value = cleanMoney };
            WetMoney = new() { Value = wetMoney };
            DirtyMoney = new() { Value = dirtyMoney };
            Cartridges = new() { Value = cartridges };
            Paper = new() { Value = paper };
            
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
