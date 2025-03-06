using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Revelator.io24.Api;
using Revelator.io24.StreamDeck.Actions.Enums;
using Revelator.io24.StreamDeck.Extensions;
using Revelator.io24.Api.Models.ValueConverters;

namespace Revelator.io24.StreamDeck.Actions.Encoders.OutputLevel;

[PluginActionId("com.oddbear.revelator.io24.encoder.output-level")]
public class OutputLevelEncoder : HybridBase
{
    private readonly OutputLevelCache _outputLevelCache;
    private readonly Device _device;
    private readonly bool _isEncoder;

    // Warning, settings is mutable:
    private readonly OutputLevelEncoderSettings _settings = new ();

    public OutputLevelEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        _isEncoder = payload.Controller == "Encoder";

        _outputLevelCache = Program.OutputLevelCache;
        _device = Program.Device;

        if (payload.Settings?.Count > 0)
        {
            _settings = payload.Settings.ToObject<OutputLevelEncoderSettings>()!;
            _ = StatesUpdated();
            _ = RefreshState();
        }

        _outputLevelCache.PropertyChanged += CachedPropertyChanged;
        _device.Main.PropertyChanged += SkipCachePropertyChanged;
    }

    public override void Dispose()
    {
        _outputLevelCache.PropertyChanged -= CachedPropertyChanged;
        _device.Main.PropertyChanged -= SkipCachePropertyChanged;
    }

    private void CachedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Important use this event instead of calling RefreshState directly when changing state.
        // If not the state will be updated locally and not globally.
        _ = StatesUpdated();
    }

    private void SkipCachePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Skip cache if it's a state change, as this can only be done physically on the device.
        // Only update the state, nothing more.
        _ = RefreshState();
    }

    public override Task KeyPressedAsync(KeyPayload payload)
    {
        if (!ValidKeyPad(out var output, out var action))
            return Task.CompletedTask;

        switch (action)
        {
            case VolumeActionType.Set:
                KeypadSet(output);
                break;
            case VolumeActionType.Adjust:
                KeypadAdjust(output);
                break;
        }

        return Task.CompletedTask;
    }

    public override async Task KeyReleasedAsync(KeyPayload payload)
    {
        // Hack because on release StreamDeck sets the state automatically:
        await RefreshState();
    }

    private void KeypadSet(DeviceOut output)
    {
        switch (output)
        {
            case DeviceOut.Blend:
                _outputLevelCache.MonitorBlend = new BlendValue { Blend = _settings.SetBlend };
                break;
            case DeviceOut.Phones:
                _outputLevelCache.HeadphonesVolume = new OutputValue { Db = _settings.SetVolume };
                break;
            case DeviceOut.MainOut:
                _outputLevelCache.MainOutVolume = new OutputValue { Db = _settings.SetVolume };
                break;
        }
    }

    private void KeypadAdjust(DeviceOut output, int ticks = 1)
    {
        switch (output)
        {
            case DeviceOut.Blend:
                // I get about 30 ticks for the whole dial on the interface from -1 to +1
                _outputLevelCache.MonitorBlend = new BlendValue { Blend = _outputLevelCache.MonitorBlend.Blend + _settings.AdjustBlend };
                return;
            case DeviceOut.Phones:
                _outputLevelCache.HeadphonesVolume = new OutputValue { Db = _outputLevelCache.HeadphonesVolume.Db + _settings.AdjustVolume };
                return;
            case DeviceOut.MainOut:
                // 0 -> -0.06 -> -0.12
                // -10 -> -9.52 -> -9.07
                // -96 -> -91.9 -> -87.97 -> -84.21
                _outputLevelCache.MainOutVolume = new OutputValue { Db = _outputLevelCache.MainOutVolume.Db + _settings.AdjustVolume };
                return;
        }
    }

    public override Task DialRotateAsync(DialRotatePayload payload)
    {
        if (!ValidEncoder(out var output))
            return Task.CompletedTask;

        // We use fixed values, as these are the same as the ones in UC.
        switch (output)
        {
            case DeviceOut.Blend:
                _outputLevelCache.MonitorBlend = new BlendValue { Percent = _outputLevelCache.MonitorBlend.Percent + payload.Ticks };
                break;
            case DeviceOut.Phones:
                _outputLevelCache.HeadphonesVolume = new OutputValue { Percent = _outputLevelCache.HeadphonesVolume.Percent + payload.Ticks };
                break;
            case DeviceOut.MainOut:
                _outputLevelCache.MainOutVolume = new OutputValue { Percent = _outputLevelCache.MainOutVolume.Percent + payload.Ticks };
                break;
        }

        return Task.CompletedTask;
    }

    public override async Task ReceivedSettingsAsync(ReceivedSettingsPayload payload)
    {
        Tools.AutoPopulateSettings(_settings, payload.Settings);

        // To update the displays:
        await StatesUpdated();
        // If we go from or to main:
        await RefreshState();
    }

    private async Task StatesUpdated()
    {
        // We don't need action here:
        if (!ValidEncoder(out var output))
            return;

        switch (output)
        {
            case DeviceOut.Blend:
                await RefreshBlend(_outputLevelCache.MonitorBlend);
                return;
            case DeviceOut.Phones:
                await RefreshVolume(_outputLevelCache.HeadphonesVolume);
                return;
            case DeviceOut.MainOut:
                await RefreshVolume(_outputLevelCache.MainOutVolume);
                return;
        }
    }

    private async Task RefreshBlend(BlendValue blendValue)
    {
        if (_isEncoder)
        {
            await Connection.SetFeedbackAsync(JObject.FromObject(new
            {
                value = $"{blendValue.Blend:0.00}",
                indicator = blendValue.Percent
            }));
        }
        else
        {
            await Connection.SetTitleAsync($"{blendValue.Blend:0.00}");
        }
    }

    private async Task RefreshVolume(OutputValue outputValue)
    {
        if (_isEncoder)
        {
            await Connection.SetFeedbackAsync(JObject.FromObject(new
            {
                value = $"{outputValue.Db:0.00} dB",
                indicator = outputValue.Percent
            }));
        }
        else
        {
            await Connection.SetTitleAsync($"{outputValue.Db:0.00} dB");
        }
    }

    private async Task RefreshState()
    {
        // We don't need action here:
        if (!ValidEncoder(out var output))
            return;

        switch (output)
        {
            case DeviceOut.Blend:
            case DeviceOut.Phones:
                await Connection.SetStateAsync(false);
                return;
            default:
                await Connection.SetStateAsync(_device.Main.HardwareMute);
                return;
        }
    }

    private bool ValidKeyPad(out DeviceOut output, out VolumeActionType action)
    {
        output = _settings.Output ?? default;
        action = _settings.Action ?? default;

        return _settings is
        {
            Output: not null,
            Action: not null
        };
    }

    private bool ValidEncoder(out DeviceOut output)
    {
        output = _settings.Output ?? default;

        return _settings.Output is not null;
    }

    #region NotUsed

    public override Task DialDownAsync(DialPayload payload)
    {
        // We don't have an action here.
        return Task.CompletedTask;
    }

    public override Task TouchPressAsync(TouchpadPressPayload payload)
    {
        // We don't have an action here.
        return Task.CompletedTask;
    }

    #endregion
}
