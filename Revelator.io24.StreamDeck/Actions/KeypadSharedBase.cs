using BarRaider.SdTools;

namespace Revelator.io24.StreamDeck.Actions;

public abstract class KeypadSharedBase : KeypadBase
{
    protected KeypadSharedBase(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        //
    }

    public override async void KeyPressed(KeyPayload payload)
    {
        try
        {
            await KeyPressedAsync(payload);
        }
        catch (Exception e)
        {
            // LOG
        }
    }

    public abstract Task KeyPressedAsync(KeyPayload payload);

    public override async void KeyReleased(KeyPayload payload)
    {
        try
        {
            await KeyReleasedAsync(payload);
        }
        catch (Exception e)
        {
            // LOG
        }
    }

    public abstract Task KeyReleasedAsync(KeyPayload payload);

    public override async void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        try
        {
            await ReceivedSettingsAsync(payload);
        }
        catch (Exception e)
        {
            // LOG
        }
    }

    public abstract Task ReceivedSettingsAsync(ReceivedSettingsPayload payload);

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
        // Ignore for now
    }

    public override void OnTick()
    {
        // Ignore for now
    }

    public override void Dispose()
    {
        // Ignore for now
    }
}
