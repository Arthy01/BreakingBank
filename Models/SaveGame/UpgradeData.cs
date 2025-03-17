namespace BreakingBank.Models.SaveGame
{
    public class UpgradeData : SaveGameData
    {
        public List<DirtyField<Upgrade>> Upgrades { get; } = new List<DirtyField<Upgrade>>()
        {
            new DirtyField<Upgrade>{ Value = new Upgrade("Test Upgrade", "Test Description", 100, 50, 1) }
        };

        public UpgradeData()
        {
            Upgrades[0].OnDirtyStateChanged += () => HandleDirtyStateChanged(Upgrades[0], Upgrades[0].Value!.Name);
            Upgrades[0].Value!.OnDirtyStateChanged += () => { if (Upgrades[0].Value!.Level.IsDirty) Upgrades[0].SetDirty(); };
        }

        public override void ClearDirtyData()
        {
            Upgrades[0].ClearDirty();
            Upgrades[0].Value!.ClearDirtyData();

            base.ClearDirtyData();
        }
    }
}
