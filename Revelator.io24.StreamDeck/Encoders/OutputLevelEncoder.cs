using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using Revelator.io24.Api;
using Revelator.io24.StreamDeck.Settings;
using System.Diagnostics;
using System.Globalization;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Revelator.io24.Api.Models.Global;

namespace Revelator.io24.StreamDeck.Encoders;

[PluginActionId("com.oddbear.revelator.io24.outputratio")]
public class OutputLevelEncoder : EncoderBase
{
    private OutputLevelDialSettings _settings;

    private readonly Device _device;

    public OutputLevelEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        _device = Program.Device;
        _settings ??= new OutputLevelDialSettings();

        if (payload.Settings?.Count > 0)
        {
            RefreshSettings(payload.Settings);
        }

        _device.Global.PropertyChanged += PropertyChanged;
    }

    public override void Dispose()
    {
        _device.Global.PropertyChanged -= PropertyChanged;
    }

    public override void DialRotate(DialRotatePayload payload)
    {
        var value = GetVolumeOrRatio();

        value += payload.Ticks;

        if (value is < 0 or > 100)
            return;

        SetVolumeOrRatio(value);
    }

    public override void DialDown(DialPayload payload)
    {
        // Press Down
    }

    public override void DialUp(DialPayload payload)
    {
        // Press Up
    }

    public override void TouchPress(TouchpadPressPayload payload)
    {
        // Touch Screen
    }

    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        RefreshSettings(payload.Settings);
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
        RefreshSettings(payload.Settings);
    }

    public override void OnTick()
    {

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

    private async void RefreshSettings(JObject settings)
    {
        try
        {
            _settings = settings.ToObject<OutputLevelDialSettings>()!;

            await UpdateOutputImage(_settings.DeviceOut);
            await UpdateOutputTitle(_settings.DeviceOut);
            await UpdateOutputFeedback(_settings.DeviceOut);
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
                case nameof(Global.MainOutVolume):
                    VolumeUpdated(DeviceOut.MainOut);
                    break;
                case nameof(Global.HeadphonesVolume):
                    VolumeUpdated(DeviceOut.Phones);
                    break;
                case nameof(Global.MonitorBlend):
                    VolumeUpdated(DeviceOut.Blend);
                    break;
                default:
                    return;
            }
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async void VolumeUpdated(DeviceOut deviceOut)
    {
        try
        {
            await UpdateOutputFeedback(deviceOut);
            await UpdateOutputTitle(deviceOut);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private async Task UpdateOutputImage(DeviceOut deviceOut)
    {
        var outputImageName = GetImageNameFromInput(deviceOut);
        await SetImageStates($"{outputImageName}_on");
    }

    private async Task UpdateOutputFeedback(DeviceOut deviceOut)
    {
        var volumeInPercentage = GetVolumeOrRatio();

        var dkv = new Dictionary<string, string>
        {
            // Output Source:
            ["title"] = deviceOut.ToString(),
            // Volume Title in percentage:
            ["value"] = $"{volumeInPercentage} %",
            // Volume bar in percentage 0-100:
            ["indicator"] = volumeInPercentage.ToString(CultureInfo.InvariantCulture)
        };

        await Connection.SetFeedbackAsync(dkv);
    }

    private async Task UpdateOutputTitle(DeviceOut deviceOut)
    {
        var volumeInPercentage = GetVolumeOrRatio();
        await Connection.SetTitleAsync($"{deviceOut}: {volumeInPercentage} %");
    }

    private async Task SetImageStates(string imageName)
    {
        try
        {
            var onImageBytes = await File.ReadAllBytesAsync($"./Images/Plugin/{imageName}.png");
            var onBase64 = Convert.ToBase64String(onImageBytes);
            await Connection.SetImageAsync("data:image/png;base64," + onBase64);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private string GetImageNameFromInput(DeviceOut input)
    {
        switch (input)
        {
            case DeviceOut.Phones:
                return "headphones";
            case DeviceOut.Blend:
            case DeviceOut.MainOut:
            default:
                return "output";
        }
    }
}
