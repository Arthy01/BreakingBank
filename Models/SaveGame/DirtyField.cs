namespace BreakingBank.Models.SaveGame
{
    public class DirtyField<T>
    {
        public event Action? OnDirtyStateChanged;

        public bool IsDirty { get; private set; } = true;

        public T? Value
        {
            get => _value;
            set
            {
                if (!Equals(_value, value)) 
                { 
                    _value = value;
                    IsDirty = true;
                    OnDirtyStateChanged?.Invoke();
                }
            }
        }

        private T? _value;

        public void ClearDirty()
        {
            IsDirty = false;
            OnDirtyStateChanged?.Invoke();
        }

        public void SetDirty()
        {
            IsDirty = false;
            OnDirtyStateChanged?.Invoke();
        }
    }
}
