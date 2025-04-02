
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

        public enum Resource
        {
            CleanMoney,
            WetMoney,
            DirtyMoney,
            Cartridges, 
            Paper
        }

        public ulong GetResourceAmount(Resource resource)
        {
            switch (resource)
            {
                case Resource.CleanMoney:
                    return CleanMoney.Value;
                case Resource.WetMoney:
                    return WetMoney.Value;
                case Resource.DirtyMoney:
                    return DirtyMoney.Value;
                case Resource.Cartridges:
                    return Cartridges.Value;
                case Resource.Paper:
                    return Paper.Value;
                default:
                    throw new NotImplementedException();
            }
        }

        public void AddResource(Resource resource, ulong amount)
        {
            switch (resource)
            {
                case Resource.CleanMoney:
                    CleanMoney.Value += amount;
                    return;
                case Resource.WetMoney:
                    WetMoney.Value += amount;
                    return;
                case Resource.DirtyMoney:
                    DirtyMoney.Value += amount;
                    return;
                case Resource.Cartridges:
                    Cartridges.Value += amount;
                    return;
                case Resource.Paper:
                    Paper.Value += amount;
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        public void RemoveResource(Resource resource, ulong amount)
        {
            if (GetResourceAmount(resource) < amount)
                return;

            switch (resource)
            {
                case Resource.CleanMoney:
                    CleanMoney.Value -= amount;
                    return;
                case Resource.WetMoney:
                    WetMoney.Value -= amount;
                    return;
                case Resource.DirtyMoney:
                    DirtyMoney.Value -= amount;
                    return;
                case Resource.Cartridges:
                    Cartridges.Value -= amount;
                    return;
                case Resource.Paper:
                    Paper.Value -= amount;
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

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
