namespace BreakingBank.Models.SaveGame
{
    public class ProcessingData : SaveGameData
    {
        public DirtyField<ProcessingUnit> Printers { get; } = new() { Value = new ProcessingUnit() };
        public DirtyField<ProcessingUnit> WashingMachines { get; } = new() { Value = new ProcessingUnit() };
        public DirtyField<ProcessingUnit> Dryers { get; } = new() { Value = new ProcessingUnit() };

        public ProcessingData()
        {
            Printers.OnDirtyStateChanged += () => HandleDirtyStateChanged(Printers, nameof(Printers));
            WashingMachines.OnDirtyStateChanged += () => HandleDirtyStateChanged(WashingMachines, nameof(WashingMachines));
            Dryers.OnDirtyStateChanged += () => HandleDirtyStateChanged(Dryers, nameof(Dryers));
        }

        public override void ClearDirtyData()
        {
            Printers.ClearDirty();
            Printers.Value!.ClearDirtyData();

            WashingMachines.ClearDirty();
            WashingMachines.Value!.ClearDirtyData();

            Dryers.ClearDirty();
            Dryers.Value!.ClearDirtyData();

            base.ClearDirtyData();
        }
    }
}
