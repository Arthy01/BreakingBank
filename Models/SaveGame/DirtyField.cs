using System.Text.Json.Serialization;

namespace BreakingBank.Models.SaveGame
{
    public class DirtyField<T>
    {
        public event Action? OnDirtyStateChanged;

        public bool IsDirty { get; private set; } = true;

        private readonly object _lock = new();

        public T? Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value)) 
                {
                    lock (_lock)
                    {
                        _value = value;
                        IsDirty = true;
                    }
                    OnDirtyStateChanged?.Invoke();
                }
            }
        }

        private T? _value;

        public void ClearDirty()
        {
            lock (_lock)
            {
                IsDirty = false;
            }

            OnDirtyStateChanged?.Invoke();
        }

        public void SetDirty()
        {
            lock (_lock)
            {
                IsDirty = true;
            }

            OnDirtyStateChanged?.Invoke();
        }
    }
}
