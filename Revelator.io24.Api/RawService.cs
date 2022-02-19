using System.Text.Json;

namespace Revelator.io24.Api
{
    public class RawService
    {
        public delegate void SyncronizeEvent();
        public delegate void ValueStateEvent(string route, float value);
        public delegate void StringStateEvent(string route, string value);

        private Dictionary<string, float> _values = new();
        private Dictionary<string, string> _string = new();
        private Dictionary<string, string[]> _strings = new();

        public event SyncronizeEvent Syncronized;
        public event ValueStateEvent ValueStateUpdated;
        public event StringStateEvent StringStateUpdated;

        internal Action<string, float> SetValueMethod;

        public void SetValue(string route, float value)
        {
            //TODO: Refactor... We will need to split listen and send for that to work.
            SetValueMethod?.Invoke(route, value);
        }

        public float GetValue(string route)
            => _values.TryGetValue(route, out var value)
                ? value : default;

        public string? GetString(string route)
            => _string.TryGetValue(route, out var value)
                ? value : default;

        internal void UpdateValueState(string route, float value)
        {
            _values[route] = value;
            ValueStateUpdated?.Invoke(route, value);
        }

        internal void UpdateStringState(string route, string value)
        {
            _string[route] = value;
            StringStateUpdated?.Invoke(route, value);
        }

        internal void UpdateStringsState(string route, string[] values)
        {

        }

        internal void Syncronize(string json)
        {
            var doc = JsonSerializer.Deserialize<JsonDocument>(json);
            if (doc is null)
                return;

            var id = doc.RootElement.GetProperty("id").GetString();
            var children = doc.RootElement.GetProperty("children");
            var shared = doc.RootElement
                .GetProperty("shared")
                .GetProperty("strings")
                .EnumerateArray()
                .First()
                .EnumerateArray()
                .Select(item => item.GetString())
                .ToArray();

            Traverse(children, string.Empty);

            Syncronized?.Invoke();
        }


        private void Traverse(JsonElement element, string path)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    _values[path] = element.GetSingle();
                    return;
                case JsonValueKind.String:
                    _string[path] = element.GetString() ?? string.Empty;
                    return;
                case JsonValueKind.Array:
                    var array = element.EnumerateArray();
                    _strings[path] = array
                        .Select(item => item.GetString() ?? string.Empty)
                        .Where(str => str != string.Empty)
                        .ToArray();
                    return;
                case JsonValueKind.Object:
                    TraverseObject(element, path);
                    return;
                default:
                    //TODO: Logging, what is going on here?
                    return;
            }
        }

        private void TraverseObject(JsonElement objectElement, string path)
        {
            var properties = objectElement.EnumerateObject();
            foreach (var property in properties)
            {
                switch (property.Name)
                {
                    //Theese we can get from the ValueKind, should just be passed up with no '/' added.
                    case "children":
                    case "values":
                    case "ranges":
                    case "strings":
                        Traverse(property.Value, CreatePath(path));
                        continue;
                    default:
                        Traverse(property.Value, CreatePath(path, property.Name));
                        continue;
                }
            }
        }

        private string CreatePath(string path, string? propertyName = null)
        {
            //Path should never start with a '/'
            if (path is null || path.Length < 1)
                return $"{path}{propertyName}";

            //Path already ends with '/', should not add another one
            if (path[^1] == '/')
                return $"{path}{propertyName}";

            //Add '/' to path:
            return $"{path}/{propertyName}";
        }
    }
}
