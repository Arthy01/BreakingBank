using System.Text.Json.Serialization;

namespace BreakingBank.Models.SaveGame
{
    public abstract class SaveGameData
    {
        public event Action? OnDirtyStateChanged;

        [JsonIgnore]
        public IReadOnlyDictionary<string, object> DirtyData => _dirtyData;

        private Dictionary<string, object> _dirtyData = new();

        public virtual void ClearDirtyData()
        {
            _dirtyData.Clear();

            OnDirtyStateChanged?.Invoke();
        }

        protected void HandleDirtyStateChanged<T>(DirtyField<T> field, string fieldName)
        {
            if (!field.IsDirty)
                return;

            if (field.Value == null)
                return;

            _dirtyData[fieldName.ToLower()] = field;

            OnDirtyStateChanged?.Invoke();
        }


    }
}
