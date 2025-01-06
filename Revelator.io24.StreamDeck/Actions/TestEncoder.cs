using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime;

namespace Revelator.io24.StreamDeck.Actions;

public class TestEncoderSettings
{
    [JsonProperty(PropertyName = "testValue")]
    public string TestValue { get; set; } = "?";
}

// Just a dummy test encoder:
/*
    {
     "Name": "Test",
     "UUID": "com.oddbear.revelator.io24.encoder.test",
     "PropertyInspectorPath": "PropertyInspector/TestEncoder.html",
     "Controllers": [ "Encoder", "Keypad" ],
     "Icon": "Images/Icons/Icon",
      "Encoder": {
        "layout": "$B1",
        "TriggerDescription": {
          "Rotate": "Increase/Decrease",
          "Push": "Toggle mute"
        }
      },
     "States": [
       {
         "Image": "Images/Icons/Image"
       }
     ]
   },
 */
[PluginActionId("com.oddbear.revelator.io24.encoder.test")]
public class TestEncoder : EncoderSharedBase<TestEncoderSettings>, IKeypadPlugin
{
    private enum Controller
    {
        Keypad,
        Encoder
    }

    private Controller _controller;

    public TestEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        _controller = Enum.Parse<Controller>(payload.Controller);

        Connection.OnPropertyInspectorDidAppear += ConnectionOnOnPropertyInspectorDidAppear;
    }

    private async void ConnectionOnOnPropertyInspectorDidAppear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
    {
        try
        {
            // Need some JS hacking, but seems to work, not sure why I need to send a message back though.
            await Connection.SendToPropertyInspectorAsync(JObject.FromObject(new
            {
                showOtherOptions = true
            }));
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    public override void Dispose()
    {
        //
    }

    public override void DialRotate(DialRotatePayload payload)
    {
        //
    }

    public override void DialDown(DialPayload payload)
    {
        //
    }

    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        //
    }

    // This does work:
    public async void KeyPressed(KeyPayload payload)
    {
        try
        {
            // Does not trigger event in PI, but changes in the PI does trigger event.
            _settings.TestValue = Random.Shared.Next().ToString();
            var jSettings = JObject.FromObject(_settings);
            await Connection.SetSettingsAsync(jSettings);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    public void KeyReleased(KeyPayload payload)
    {
        //
    }
}
