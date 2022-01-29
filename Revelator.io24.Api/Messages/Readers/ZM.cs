using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Models.Json;
using Serilog;
using System.Text;
using System.Text.Json;

namespace Revelator.io24.Api.Messages.Readers
{
    public static class ZM
    {
        /// <summary>
        /// ZLib compressed json message
        /// </summary>
        public static string GetJsonMessage(byte[] data)
        {
            var header = data[0..4];
            var messageLength = data[4..6];
            var messageType = data[6..8];
            var customBytes = data[8..12];

            var size = BitConverter.ToInt32(data[12..16]);

            //ZLib Message:
            using var compressedStream = new MemoryStream(data[16..]);
            using var inputStream = new InflaterInputStream(compressedStream);
            using var outputStream = new MemoryStream();

            inputStream.CopyTo(outputStream);
            outputStream.Position = 0;

            return Encoding.ASCII.GetString(outputStream.ToArray());
        }

        public static SynchronizeModel? GetSynchronizeModel(string json)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var synchronize = JsonSerializer.Deserialize<Synchronize>(json, options);
            if (synchronize is null)
                return null;

            TraverseExtensionData(synchronize);

            return new SynchronizeModel(synchronize);
        }

        private static void TraverseExtensionData(object obj)
        {
            try
            {
                if (obj is null)
                    return;

                var type = obj.GetType();
                var extensionDataProperty = type.GetProperty("ExtensionData");
                if (extensionDataProperty is null)
                {
                    Log.Warning("[{className}] ExtensionData property was missing on type '{typeName}'", nameof(ZM), type.Name);
                    return;
                }

                var extensionData = extensionDataProperty?.GetValue(obj) as Dictionary<string, object>;

                if (extensionData?.Any() == true)
                {
                    var unknownProperties = string.Join(", ", extensionData.Keys);
                    Log.Warning("[{className}] Type '{typeName}' had unknown properties: '{unknownProperties}'", nameof(ZM), type.Name, unknownProperties);
                }

                var properties = type.GetProperties()
                    .Where(property => property != extensionDataProperty)
                    .Where(property => !property.PropertyType.IsValueType)
                    .Where(property => !property.PropertyType.IsArray)
                    .Where(property => property.PropertyType != typeof(string));

                foreach (var property in properties)
                {
                    var value = property.GetValue(obj);
                    if (value is not null)
                        TraverseExtensionData(value);
                }
            }
            catch (Exception exception)
            {
                Log.Error("[{className}] {exception}", nameof(ZM), exception);
            }
        }
    }
}
