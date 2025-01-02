using System.Diagnostics;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using Revelator.io24.StreamDeck.Settings;
using System.ComponentModel;
using Revelator.io24.Api.Models.Global;

namespace Revelator.io24.StreamDeck.Actions;

[PluginActionId("com.oddbear.revelator.io24.outputratio")]
public class OutputLevelAction : KeypadBase
{
    private OutputLevelSettings _settings;

    private readonly Device _device;

    public OutputLevelAction(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device = Program.Device;
        _settings ??= new OutputLevelSettings();

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
        var value = GetVolumeOrRatio();
        switch (_settings.ChangeType)
        {
            case VolumeType.Increment:
                value += _settings.Value;
                SetVolumeOrRatio(value);
                break;
            case VolumeType.Decrement:
                value -= _settings.Value;
                SetVolumeOrRatio(value);
                break;
            case VolumeType.Absolute:
            default:
                value = _settings.Value;
                SetVolumeOrRatio(value);
                break;
        }
    }

    public override void KeyReleased(KeyPayload payload)
    {
        //
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
        //
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
            _settings = settings.ToObject<OutputLevelSettings>()!;

            await UpdateOutputImage(_settings.DeviceOut);
            await UpdateOutputTitle(_settings.DeviceOut);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }
    }

    private void PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            switch (e.PropertyName)
            {
                case nameof(Global.MainOutVolume) when _settings.DeviceOut == DeviceOut.MainOut:
                    VolumeUpdated(DeviceOut.MainOut);
                    break;
                case nameof(Global.HeadphonesVolume) when _settings.DeviceOut == DeviceOut.Phones:
                    VolumeUpdated(DeviceOut.Phones);
                    break;
                case nameof(Global.MonitorBlend) when _settings.DeviceOut == DeviceOut.Blend:
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

    private async Task UpdateOutputTitle(DeviceOut deviceOut)
    {
        var volumeInPercentage = GetVolumeOrRatio();
        var title = GetTitleFromInput(deviceOut);
        await Connection.SetTitleAsync($"{title}: {volumeInPercentage} %");
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

    private string GetTitleFromInput(DeviceOut input)
    {
        switch (input)
        {
            case DeviceOut.Phones:
                return "Phones";
            case DeviceOut.Blend:
                return "Blend";
            case DeviceOut.MainOut:
            default:
                return "Main";
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