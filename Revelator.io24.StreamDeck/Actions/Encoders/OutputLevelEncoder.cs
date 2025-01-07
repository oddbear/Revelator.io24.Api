using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using System.Diagnostics;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Revelator.io24.Api.Models.Global;
using Revelator.io24.StreamDeck.Helper;
using Revelator.io24.StreamDeck.Actions.Encoders.Settings;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;
using Revelator.io24.StreamDeck.Actions.Enums;

namespace Revelator.io24.StreamDeck.Actions.Encoders;

[PluginActionId("com.oddbear.revelator.io24.encoder.output-level")]
public class OutputLevelEncoder : EncoderSharedBase<OutputLevelEncoderSettings>
{
    private readonly Controller _controller;

    public OutputLevelEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        _controller = Enum.Parse<Controller>(payload.Controller);

        // Empty if no settings are changed (default settings not picked up)
        RefreshSettings(payload.Settings);

        _device.Global.PropertyChanged += PropertyChanged;
        Connection.OnPropertyInspectorDidAppear += ConnectionOnOnPropertyInspectorDidAppear;
    }

    public override void Dispose()
    {
        _device.Global.PropertyChanged -= PropertyChanged;
        Connection.OnPropertyInspectorDidAppear -= ConnectionOnOnPropertyInspectorDidAppear;
    }

    public override void KeyPressed(KeyPayload payload)
    {
        switch (_settings.DeviceOut)
        {
            case DeviceOut.Blend:
                _device.Global.MonitorBlend = KeyPressedBlend(_device.Global.MonitorBlend);
                return;
            case DeviceOut.Phones:
                _device.Global.HeadphonesVolume = KeyPressedOutput(_device.Global.HeadphonesVolume, _settings.Value);
                return;
            case DeviceOut.MainOut:
                _device.Global.MainOutVolume = KeyPressedOutput(_device.Global.MainOutVolume, _settings.Value);
                return;
        }
    }

    private int KeyPressedOutput(int oldValueP, float ticks)
    {
        // oldValueP is 0% - 100%, but we need to work with native percentages 0f - 1f:
        var oldValuePNative = oldValueP / 100f;

        var oldValueDb = LookupTable.OutputPercentageToDb(oldValuePNative);

        var newValueDb = _settings.Action switch
        {
            VolumeActionType.Adjust => oldValueDb + ticks,
            _ => ticks
        };

        if (newValueDb is < -96 or > 0)
            return oldValueP;

        // newValuePis 0f - 1f, but we need to work with 0% - 100%:
        var newValuePNative = LookupTable.OutputDbToPercentage(newValueDb);

        //-39.84 dB cannot turn up on 1 dB ticks, as both are to close in the dataset:
        if (Math.Abs(newValuePNative - oldValuePNative) < 0.01f) // Closer than 1%
            newValuePNative += ticks / 100f;

        var newValueP = (int)Math.Round(newValuePNative * 100f);
        return newValueP;
    }

    private float KeyPressedBlend(float oldValueP)
    {
        // Convert 0f - 1f to -1 - +1:
        var oldValue = oldValueP * 2f - 1f;

        var newValue = _settings.Action switch
        {
            VolumeActionType.Adjust => oldValue + _settings.Value,
            _ => _settings.Value
        };

        if (newValue is < -1 or > +1)
            return oldValueP;

        // Convert -1 - +1 to 0f - 1f:
        var newValueP = (newValue + 1f) / 2f;
        return newValueP;
    }

    public override void DialRotate(DialRotatePayload payload)
    {
        switch (_settings.DeviceOut)
        {
            case DeviceOut.Blend:
                _device.Global.MonitorBlend = DialRotateBlend(_device.Global.MonitorBlend, payload.Ticks);
                return;
            case DeviceOut.Phones:
                _device.Global.HeadphonesVolume = DialRotateOutput(_device.Global.HeadphonesVolume, payload.Ticks * _settings.Value);
                return;
            case DeviceOut.MainOut:
                _device.Global.MainOutVolume = DialRotateOutput(_device.Global.MainOutVolume, payload.Ticks * _settings.Value);
                return;
        }
    }

    private int DialRotateOutput(int oldValueP, float ticks)
    {
        // oldValueP is 0% - 100%, but we need to work with native percentages 0f - 1f:
        var oldValuePNative = oldValueP / 100f;

        var oldValueDb = LookupTable.OutputPercentageToDb(oldValuePNative);

        var newValueDb = oldValueDb + ticks;

        if (newValueDb is < -96 or > 0)
            return oldValueP;

        // newValuePis 0f - 1f, but we need to work with 0% - 100%:
        var newValuePNative = LookupTable.OutputDbToPercentage(newValueDb);

        //-39.84 dB cannot turn up on 1 dB ticks, as both are to close in the dataset:
        if (Math.Abs(newValuePNative - oldValuePNative) < 0.01f) // Closer than 1%
            newValuePNative += ticks / 100f;

        var newValueP = (int)Math.Round(newValuePNative * 100f);
        return newValueP;
    }

    private float DialRotateBlend(float oldValueP, int ticks)
    {
        // oldValueP is 0f - 1f for -1 to +1, we have to calculate the percentage.
        // Ex. -0.6 = 0.2 = 20%

        // Convert 0f - 1f to -1 - +1:
        var oldValue = oldValueP * 2f - 1f;

        var valueIncrease = ticks * _settings.Value;

        var newValue = oldValue + valueIncrease;
        if (newValue is < -1 or > +1)
            return oldValueP;

        // Convert -1 - +1 to 0f - 1f:
        var newValueP = (newValue + 1f) / 2f;
        return newValueP;
    }


    public override void DialDown(DialPayload payload)
    {
        // We don't have an action here.
    }

    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        // TODO: On change (ex Output or Action) we get the old value and not the new one.
        RefreshSettings(payload.Settings);
    }

    private async void RefreshSettings(JObject settings)
    {
        try
        {
            _settings = settings.ToObject<OutputLevelEncoderSettings>()!;
            
            await RefreshState();
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            switch (e.PropertyName)
            {
                case nameof(Global.MainOutVolume) when _settings.DeviceOut == DeviceOut.MainOut:
                case nameof(Global.HeadphonesVolume) when _settings.DeviceOut == DeviceOut.Phones:
                case nameof(Global.MonitorBlend) when _settings.DeviceOut == DeviceOut.Blend:
                    await RefreshState();
                    return;
            }
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private string GetValue(float volumeInPercentage)
    {
        return _settings.DeviceOut switch
        {
            DeviceOut.Blend => $"{volumeInPercentage / 50f - 1f:0.00}",
            _ => $"{LookupTable.OutputPercentageToDb(volumeInPercentage / 100f):0.0} dB"
        };
    }

    private float GetVolumeOrRatio()
    {
        return _settings.DeviceOut switch
        {
            DeviceOut.Blend => (int)Math.Round(_device.Global.MonitorBlend),
            DeviceOut.Phones => _device.Global.HeadphonesVolume,
            _ => _device.Global.MainOutVolume
        };
    }
    
    private async void ConnectionOnOnPropertyInspectorDidAppear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
    {
        try
        {
            var uiProperties = JObject.FromObject(new
            {
                // Will never change, as it will involve delete and create:
                isEncoder = _controller == Controller.Encoder,
                action = _settings.Action.ToString(),
                device = _settings.DeviceOut.ToString(),
                value = _settings.Value
            });

            // Need some JS hacking, but seems to work, not sure why I need to send a message back though.
            await Connection.SendToPropertyInspectorAsync(uiProperties);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    protected override async Task RefreshState()
    {
        var volumeInPercentage = GetVolumeOrRatio();

        // Feedback:
        await SetFeedbackAsync(new FeedbackCard
        {
            Value = GetValue(volumeInPercentage),
            Indicator = volumeInPercentage
        });

        // KeyPad:
        var titleValue = GetValue(volumeInPercentage);
        await Connection.SetTitleAsync(titleValue);

        switch (_settings.DeviceOut)
        {
            // If Monitor is selected, and the button has disabled it.
            case DeviceOut.MainOut:
                await Connection.SetStateAsync(_device.Main.HardwareMute ? 1u : 0u);
                break;
            default:
                await Connection.SetStateAsync(0u);
                break;
        }
    }
}
