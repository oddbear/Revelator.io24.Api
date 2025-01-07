using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using System.Diagnostics;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Revelator.io24.Api.Models.Global;
using Revelator.io24.StreamDeck.Helper;
using Revelator.io24.StreamDeck.Actions.Encoders.Settings;

namespace Revelator.io24.StreamDeck.Actions.Encoders;

[PluginActionId("com.oddbear.revelator.io24.encoder.output-level")]
public class OutputLevelEncoder : EncoderSharedBase<OutputLevelEncoderSettings>
{
    public OutputLevelEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        // Empty if no settings are changed (default settings not picked up)
        RefreshSettings(payload.Settings);

        _device.Global.PropertyChanged += PropertyChanged;
    }

    public override void Dispose()
    {
        _device.Global.PropertyChanged -= PropertyChanged;
    }

    public override void KeyPressed(KeyPayload payload)
    {
        // This is a -1 to +1 value:
        if (_settings.DeviceOut == DeviceOut.Blend)
        {
            var value = _settings.ChangeActionType switch
            {
                //VolumeType.Increment => GetVolumeOrRatio() + _settings.Value,
                //VolumeType.Decrement => GetVolumeOrRatio() - _settings.Value,
                _ => _settings.Value
            };
            SetVolumeOrRatio(value);
        }
        // Else we should work with dB values.
        else
        {
            var oldValueP = GetVolumeOrRatio();
            var oldValueDb = LookupTable.OutputPercentageToDb(oldValueP);
            var valueDb = _settings.ChangeActionType switch
            {
                //VolumeType.Increment => oldValueDb + _settings.Value,
                //VolumeType.Decrement => oldValueDb - _settings.Value,
                _ => _settings.Value
            };

            var volumeInPercentage = LookupTable.OutputDbToPercentage(valueDb);
            SetVolumeOrRatio(volumeInPercentage);
        }
    }

    public override void DialRotate(DialRotatePayload payload)
    {
        var value = GetVolumeOrRatio();

        // Feels smoother to double the ticks
        value += payload.Ticks * 2;

        if (value is < 0 or > 100)
            return;

        // TODO: Find better value:
        SetVolumeOrRatio(value / 100f);
    }

    public override void DialDown(DialPayload payload)
    {
        // We don't have an action here.
    }

    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        RefreshSettings(payload.Settings);
    }

    private async void RefreshSettings(JObject settings)
    {
        try
        {
            var deviceOut = _settings.DeviceOut;
            _settings = settings.ToObject<OutputLevelEncoderSettings>()!;

            // Ignore if DeviceOut is not changed:
            if (deviceOut == _settings.DeviceOut)
                return;

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

    private string GetValue(int volumeInPercentage)
    {
        return _settings.DeviceOut switch
        {
            DeviceOut.Blend => $"{volumeInPercentage / 50f - 1f:0.00}",
            _ => $"{LookupTable.OutputPercentageToDb(volumeInPercentage / 100f):0.0} dB"
        };
    }

    private int GetVolumeOrRatio()
    {
        return _settings.DeviceOut switch
        {
            DeviceOut.Blend => (int)Math.Round(_device.Global.MonitorBlend * 100f),
            DeviceOut.Phones => _device.Global.HeadphonesVolume,
            _ => _device.Global.MainOutVolume
        };
    }

    private void SetVolumeOrRatio(float value)
    {
        switch (_settings.DeviceOut)
        {
            case DeviceOut.Blend:
                _device.Global.MonitorBlend = value / 100f;
                break;
            case DeviceOut.Phones:
                _device.Global.HeadphonesVolume = (int)Math.Round(value * 100f);
                break;
            case DeviceOut.MainOut:
                _device.Global.MainOutVolume = (int)Math.Round(value * 100f);
                break;
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
