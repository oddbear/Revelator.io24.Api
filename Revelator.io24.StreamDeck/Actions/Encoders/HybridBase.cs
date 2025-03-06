using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;

namespace Revelator.io24.StreamDeck.Actions.Encoders;

public abstract class HybridBase : EncoderBase, IKeypadPlugin
{
    protected HybridBase(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        //
    }

    public override async void DialRotate(DialRotatePayload payload)
    {
        try
        {
            await DialRotateAsync(payload);
        }
        catch (Exception e)
        {
            // LOG
        }
    }

    public abstract Task DialRotateAsync(DialRotatePayload payload);

    public override async void DialDown(DialPayload payload)
    {
        try
        {
            await DialDownAsync(payload);
        }
        catch (Exception e)
        {
            // LOG
        }
    }

    public abstract Task DialDownAsync(DialPayload payload);

    public async void KeyPressed(KeyPayload payload) // Interface, so no override
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

    public async void KeyReleased(KeyPayload payload) // Interface, so no override
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

    public override void DialUp(DialPayload payload)
    {
        // Ignore for now
    }

    public abstract Task TouchPressAsync(TouchpadPressPayload payload);

    public override async void TouchPress(TouchpadPressPayload payload)
    {
        try
        {
            await TouchPressAsync(payload);
        }
        catch (Exception e)
        {
            // LOG
        }
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
