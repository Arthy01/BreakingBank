namespace BreakingBank.Models.SaveGame
{
    public class ProcessingData : SaveGameData
    {
        public DirtyField<ProcessingUnit> Printers { get; } = new() { Value = new ProcessingUnit() };
        public DirtyField<ProcessingUnit> WashingMachines { get; } = new() { Value = new ProcessingUnit() };
        public DirtyField<ProcessingUnit> Dryers { get; } = new() { Value = new ProcessingUnit() };

        public ProcessingData()
        {
            RegisterEvents();
        }

        public ProcessingData(DirtyField<ProcessingUnit> printers, DirtyField<ProcessingUnit> washingMachines, DirtyField<ProcessingUnit> dryers)
        {
            Printers = printers;
            WashingMachines = washingMachines;
            Dryers = dryers;

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            Printers.OnDirtyStateChanged += () => HandleDirtyStateChanged(Printers, nameof(Printers));
            BindSetDirtyEvents(Printers);

            WashingMachines.OnDirtyStateChanged += () => HandleDirtyStateChanged(WashingMachines, nameof(WashingMachines));
            BindSetDirtyEvents(WashingMachines);

            Dryers.OnDirtyStateChanged += () => HandleDirtyStateChanged(Dryers, nameof(Dryers));
            BindSetDirtyEvents(Dryers);
        }

        private void BindSetDirtyEvents(DirtyField<ProcessingUnit> unit)
        {
            unit.Value!.Count.OnDirtyStateChanged += () => { if (unit.Value!.Count.IsDirty) unit.SetDirty(); };
            unit.Value!.UsedCapacity.OnDirtyStateChanged += () => { if (unit.Value!.UsedCapacity.IsDirty) unit.SetDirty(); };
            unit.Value!.MaxCapacity.OnDirtyStateChanged += () => { if (unit.Value!.MaxCapacity.IsDirty) unit.SetDirty(); };
            unit.Value!.CurrentClicks.OnDirtyStateChanged += () => { if (unit.Value!.CurrentClicks.IsDirty) unit.SetDirty(); };
            unit.Value!.RequiredClicks.OnDirtyStateChanged += () => { if (unit.Value!.RequiredClicks.IsDirty) unit.SetDirty(); };
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
