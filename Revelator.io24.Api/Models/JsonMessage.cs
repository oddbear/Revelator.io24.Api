using System.Text;

namespace Revelator.io24.Api.Models
{
    public class JsonMessage
    {
        public string Json { get; set; }

        public JsonMessage(byte[] bytes)
        {
            var size = BitConverter.ToInt32(bytes[0..4]) + 4;
            Json = Encoding.ASCII.GetString(bytes[4..size]);
        }
    }
}
