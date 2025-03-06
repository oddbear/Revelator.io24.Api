using BarRaider.SdTools;

namespace Revelator.io24.StreamDeck.Extensions;

public static class ConnectionExtension
{
    public static async Task SetStateAsync(this ISDConnection connection, bool state)
    {
        await connection.SetStateAsync(state ? 1u : 0u);
    }
}
