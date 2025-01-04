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
    
    public override void DialRotate(DialRotatePayload payload)
    {
        var value = GetVolumeOrRatio();

        // Feels smoother to double the ticks
        value += payload.Ticks * 2;

        if (value is < 0 or > 100)
            return;

        SetVolumeOrRatio(value);
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

            await UpdateFeedback();
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
                    await UpdateFeedback();
                    return;
            }
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async Task UpdateFeedback()
    {
        var volumeInPercentage = GetVolumeOrRatio();

        await SetFeedbackAsync(new FeedbackCard
        {
            Value = GetValue(volumeInPercentage),
            Indicator = volumeInPercentage
        });
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

    private void SetVolumeOrRatio(int value)
    {
        switch (_settings.DeviceOut)
        {
            case DeviceOut.Blend:
                _device.Global.MonitorBlend = value / 100f;
                break;
            case DeviceOut.Phones:
                _device.Global.HeadphonesVolume = value;
                break;
            case DeviceOut.MainOut:
                _device.Global.MainOutVolume = value;
                break;
        }
    }
}
