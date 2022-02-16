using System.Text.Json;

namespace Revelator.io24.Api
{
    public class RawService
    {
        private Dictionary<string, float> _values = new();
        private Dictionary<string, string> _string = new();
        private Dictionary<string, string[]> _strings = new();

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
                    case "children":
                    case "values":
                    case "ranges":
                    case "strings":
                        if (path.Length < 1 || path[^1] == '/')
                            Traverse(property.Value, path);
                        else
                            Traverse(property.Value, $"{path}/");
                        continue;
                    default:
                        if (path.Length < 1 || path[^1] == '/')
                            Traverse(property.Value, $"{path}{property.Name}");
                        else
                            Traverse(property.Value, $"{path}/{property.Name}");
                        continue;
                }
            }
        }

    }
}
