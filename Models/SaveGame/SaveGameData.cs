using BreakingBank.Helpers;
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
            if (_dirtyData.Count == 0)
                return;

            _dirtyData.Clear();

            OnDirtyStateChanged?.Invoke();
        }

        protected void HandleDirtyStateChanged<T>(DirtyField<T> field, string fieldName)
        {
            if (!field.IsDirty)
                return;
            
            if (field.Value == null)
                return;

            var fieldType = typeof(T);

            // Falls `T` eine komplexe Klasse ist, prüfen wir die DirtyFields darin
            if (fieldType.IsClass && !(fieldType == typeof(string)))
            {
                var dirtySubFields = new Dictionary<string, object>();

                foreach (var property in fieldType.GetProperties())
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(DirtyField<>))
                    {
                        var subField = property.GetValue(field.Value); // Hole das DirtyField<T> für die Property
                        if (subField != null)
                        {
                            dynamic dynamicField = subField; // Ermöglicht Zugriff auf `IsDirty`
                            if (dynamicField.IsDirty)
                            {
                                dirtySubFields[property.Name.ToCamelCase()] = dynamicField.Value; // Speichern nur den geänderten Wert
                            }
                        }
                    }
                }

                // Falls keine Subfelder dirty sind, speichern wir nichts
                if (!dirtySubFields.Any())
                    return;

                // Speichere nur die wirklich dirty Subfelder
                _dirtyData[fieldName.ToCamelCase()] = dirtySubFields;
            }
            else
            {
                // Wenn es ein primitiver Typ ist, speichern wir ihn direkt
                _dirtyData[fieldName.ToCamelCase()] = field.Value;
            }

            OnDirtyStateChanged?.Invoke();
        }
    }
}
