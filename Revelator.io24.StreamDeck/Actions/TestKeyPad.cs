using Revelator.io24.StreamDeck.Actions.Keypads.Settings;
using BarRaider.SdTools;

namespace Revelator.io24.StreamDeck.Actions;

// Just a dummy test keypad:
/*
    {
     "Name": "Test",
     "UUID": "com.oddbear.revelator.io24.test",
     "Icon": "Images/Icons/Icon",
     "States": [
       {
         "Image": "Images/Icons/Image"
       }
     ]
   },
 */
[PluginActionId("com.oddbear.revelator.io24.test")]
public class TestKeyPad : KeypadSharedBase<RouteChangeSettings>
{
    public TestKeyPad(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        //
    }

    public override void Dispose()
    {
    }

    public override void KeyPressed(KeyPayload payload)
    {
        // Actually works... but how?
        //_device.MicrohoneLeft.HardwareMute = !_device.MicrohoneLeft.HardwareMute;
    }

    protected override Task SettingsUpdated()
    {
        return Task.CompletedTask;
    }

    protected override Task RefreshState()
    {
        return Task.CompletedTask;
    }
}
