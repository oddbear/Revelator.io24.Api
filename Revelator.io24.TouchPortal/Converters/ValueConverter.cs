using Revelator.io24.Api.Enums;

namespace Revelator.io24.TouchPortal.Converters
{
    public static class ValueConverter
    {
        public static Value GetValue(string action)
            => action switch
            {
                "Toggle" => Value.Toggle,
                "Turn On" => Value.On,
                "Turn Off" => Value.Off,
                _ => throw new InvalidOperationException()
            };
    }
}
