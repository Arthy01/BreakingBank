﻿namespace BreakingBank.Models.SaveGame
{
    public class ProcessingUnit : SaveGameData
    {
        public DirtyField<ulong> Count { get; } = new();
        public DirtyField<ulong> UsedCapacity { get; } = new();
        public DirtyField<ulong> MaxCapacity { get; } = new();
        public DirtyField<ulong> CurrentClicks { get; } = new();
        public DirtyField<ulong> RequiredClicks { get; } = new();

        public ProcessingUnit()
        {
            Count.OnDirtyStateChanged += () => HandleDirtyStateChanged(Count, nameof(Count));
            UsedCapacity.OnDirtyStateChanged += () => HandleDirtyStateChanged(UsedCapacity, nameof(UsedCapacity));
            MaxCapacity.OnDirtyStateChanged += () => HandleDirtyStateChanged(MaxCapacity, nameof(MaxCapacity));
            CurrentClicks.OnDirtyStateChanged += () => HandleDirtyStateChanged(CurrentClicks, nameof(CurrentClicks));
            RequiredClicks.OnDirtyStateChanged += () => HandleDirtyStateChanged(RequiredClicks, nameof(RequiredClicks));
        }


        public override void ClearDirtyData()
        {
            Count.ClearDirty();
            UsedCapacity.ClearDirty();
            MaxCapacity.ClearDirty();
            CurrentClicks.ClearDirty();
            RequiredClicks.ClearDirty();

            base.ClearDirtyData();
        }
    }
}
