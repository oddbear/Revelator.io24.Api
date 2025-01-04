using System.Diagnostics;
using Revelator.io24.Api.Enums;
using BarRaider.SdTools;
using System.ComponentModel;
using Revelator.io24.Api.Models.Global;
using Revelator.io24.Api.Models.Outputs;
using Revelator.io24.StreamDeck.Actions.Keypads.Settings;
using Revelator.io24.StreamDeck.Helper;

namespace Revelator.io24.StreamDeck.Actions.Keypads;

[PluginActionId("com.oddbear.revelator.io24.outputratio")]
public class OutputLevelKeypad : KeypadSharedBase<OutputLevelSettings>
{
    public OutputLevelKeypad(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device.Global.PropertyChanged += PropertyChanged;
        _device.Main.PropertyChanged += PropertyChanged;
    }

    public override void Dispose()
    {
        _device.Global.PropertyChanged -= PropertyChanged;
        _device.Main.PropertyChanged -= PropertyChanged;
    }
    
    public override void KeyPressed(KeyPayload payload)
    {
        // This is a -1 to +1 value:
        if (_settings.DeviceOut == DeviceOut.Blend)
        {
            var value = _settings.ChangeType switch
            {
                VolumeType.Increment => GetVolumeOrRatio() + _settings.Value,
                VolumeType.Decrement => GetVolumeOrRatio() - _settings.Value,
                _ => _settings.Value
            };
            SetVolumeOrRatio(value);
        }
        // Else we should work with dB values.
        else
        {
            var oldValueP = GetVolumeOrRatio();
            var oldValueDb = LookupTable.OutputPercentageToDb(oldValueP);
            var valueDb = _settings.ChangeType switch
            {
                VolumeType.Increment => oldValueDb + _settings.Value,
                VolumeType.Decrement => oldValueDb - _settings.Value,
                _ => _settings.Value
            };

            var volumeInPercentage = LookupTable.OutputDbToPercentage(valueDb);
            SetVolumeOrRatio(volumeInPercentage);
        }
    }

    protected override async Task SettingsUpdated()
    {
        await RefreshState();
    }

    private float GetVolumeOrRatio()
    {
        return _settings.DeviceOut switch
        {
            DeviceOut.Blend => (float)Math.Round(_device.Global.MonitorBlend * 100f),
            DeviceOut.Phones => _device.Global.HeadphonesVolume / 100f,
            _ => _device.Global.MainOutVolume / 100f
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

    private async void PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            switch (e.PropertyName)
            {
                case nameof(Global.MainOutVolume) when _settings.DeviceOut == DeviceOut.MainOut:
                case nameof(Global.HeadphonesVolume) when _settings.DeviceOut == DeviceOut.Phones:
                case nameof(Global.MonitorBlend) when _settings.DeviceOut == DeviceOut.Blend:
                case nameof(Main.HardwareMute):
                    await RefreshState();
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

    protected override async Task RefreshState()
    {
        var volumeInPercentage = GetVolumeOrRatio();
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

    private string GetValue(float volumeInPercentage)
    {
        return _settings.DeviceOut switch
        {
            DeviceOut.Blend => $"{volumeInPercentage / 50f - 1f:0.00}",
            _ => $"{LookupTable.OutputPercentageToDb(volumeInPercentage):0.0} dB"
        };
    }
}