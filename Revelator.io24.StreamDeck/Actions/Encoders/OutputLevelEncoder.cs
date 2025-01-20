using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Revelator.io24.Api;
using Revelator.io24.StreamDeck.Helper;
using Revelator.io24.StreamDeck.Actions.Encoders.Settings;
using Revelator.io24.StreamDeck.Actions.Enums;

namespace Revelator.io24.StreamDeck.Actions.Encoders;

[PluginActionId("com.oddbear.revelator.io24.encoder.output-level")]
public class OutputLevelEncoder : EncoderBase, IKeypadPlugin
{
    private readonly OutputLevelCache _outputLevelCache;
    private readonly Device _device;
    private readonly bool _isEncoder;

    private OutputLevelEncoderSettings _settings;

    public OutputLevelEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        _isEncoder = payload.Controller == "Encoder";

        _outputLevelCache = Program.OutputLevelCache;
        _device = Program.Device;

        if (payload.Settings == null || payload.Settings.Count == 0)
        {
            _settings = new OutputLevelEncoderSettings();
        }
        else
        {
            _settings = payload.Settings.ToObject<OutputLevelEncoderSettings>()!;
            StatesUpdated();
        }

        _outputLevelCache.PropertyChanged += VolumeCacheOnPropertyChanged;
        _device.Main.PropertyChanged += VolumeCacheOnPropertyChanged2;
    }

    public override void Dispose()
    {
        _outputLevelCache.PropertyChanged -= VolumeCacheOnPropertyChanged;
        _device.Main.PropertyChanged -= VolumeCacheOnPropertyChanged2;
    }

    private void VolumeCacheOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Important use this event instead of calling RefreshState directly when changing state.
        // If not the state will be updated locally and not globally.
        StatesUpdated();
    }

    private void VolumeCacheOnPropertyChanged2(object? sender, PropertyChangedEventArgs e)
    {
        RefreshState();
    }

    public void KeyPressed(KeyPayload payload)
    {
        switch (_settings.Action)
        {
            case VolumeActionType.Set:
                KeypadSet();
                break;
            case VolumeActionType.Adjust:
                KeypadAdjust(1);
                break;
        }
    }

    public void KeyReleased(KeyPayload payload)
    {
        // Hack because on release StreamDeck sets the state automatically:
        RefreshState();
    }

    private void KeypadSet()
    {
        switch (_settings.Output)
        {
            case DeviceOut.Blend:
                _outputLevelCache.MonitorBlend = SetBlendCalc(_settings.SetBlend);
                break;
            case DeviceOut.Phones:
                _outputLevelCache.HeadphonesVolume = SetVolumeCalc(_settings.SetVolume);
                break;
            case DeviceOut.MainOut:
                _outputLevelCache.MainOutVolume = SetVolumeCalc(_settings.SetVolume);
                break;
        }
    }

    private void KeypadAdjust(int ticks)
    {
        switch (_settings.Output)
        {
            case DeviceOut.Blend:
                // I get about 30 ticks for the whole dial on the interface from -1 to +1
                _outputLevelCache.MonitorBlend = AdjustBlendCalc(_outputLevelCache.MonitorBlend, _settings.AdjustBlend, ticks);
                return;
            case DeviceOut.Phones:
                _outputLevelCache.HeadphonesVolume = AdjustVolumeDbCalc(_outputLevelCache.HeadphonesVolume, _settings.AdjustVolume, ticks);
                return;
            case DeviceOut.MainOut:
                // 0 -> -0.06 -> -0.12
                // -10 -> -9.52 -> -9.07
                // -96 -> -91.9 -> -87.97 -> -84.21
                _outputLevelCache.MainOutVolume = AdjustVolumeDbCalc(_outputLevelCache.MainOutVolume, _settings.AdjustVolume, ticks);
                return;
        }
    }

    public override void DialRotate(DialRotatePayload payload)
    {
        // We use fixed values, as these are the same as the ones in UC.
        switch (_settings.Output)
        {
            case DeviceOut.Blend:
                _outputLevelCache.MonitorBlend = AdjustBlendCalc(_outputLevelCache.MonitorBlend, 0.02f, payload.Ticks);
                return;
            case DeviceOut.Phones:
                _outputLevelCache.HeadphonesVolume = AdjustVolumeRawCalc(_outputLevelCache.HeadphonesVolume, 0.01f, payload.Ticks);
                return;
            case DeviceOut.MainOut:
                _outputLevelCache.MainOutVolume = AdjustVolumeRawCalc(_outputLevelCache.MainOutVolume, 0.01f, payload.Ticks);
                return;
        }
    }

    private static float SetVolumeCalc(float newVolumeDb)
    {
        if (newVolumeDb < -96)
            newVolumeDb = -96;

        if (newVolumeDb > 0)
            newVolumeDb = 0;

        return LookupTable.OutputDbToPercentage(newVolumeDb);
    }

    private float AdjustVolumeDbCalc(float valueRaw, float value, int ticks)
    {
        var oldVolumeDb = LookupTable.OutputPercentageToDb(valueRaw);
        var adjustment = value * ticks;

        var newVolumeDb = oldVolumeDb + adjustment;

        if (newVolumeDb < -96)
            newVolumeDb = -96;

        if (newVolumeDb > 0)
            newVolumeDb = 0;

        return LookupTable.OutputDbToPercentage(newVolumeDb);
    }

    private float AdjustVolumeRawCalc(float valueRaw, float value, int ticks)
    {
        var adjustment = value * ticks;

        var newVolume = valueRaw + adjustment;

        if (newVolume < 0)
            newVolume = 0;

        if (newVolume > 1)
            newVolume = 1;

        return newVolume;
    }

    private static float OutputBlendToRaw(float valueBlend)
        => (valueBlend + 1) * 0.5f;

    private static float OutputRawToBlend(float valueBlend)
        => valueBlend / 0.5f - 1;

    private static float SetBlendCalc(float newBlend)
    {
        if (newBlend < -1)
            newBlend = -1;

        if (newBlend > 1)
            newBlend = 1;

        return OutputBlendToRaw(newBlend);
    }

    private static float AdjustBlendCalc(float valueRaw, float value, int ticks)
    {
        var oldBlend = OutputRawToBlend(valueRaw);

        var adjustment = value * ticks;

        var newBlend = oldBlend + adjustment;
        return SetBlendCalc(newBlend);
    }

    public override void ReceivedSettings(ReceivedSettingsPayload payload)
    {
        Tools.AutoPopulateSettings(_settings, payload.Settings);
        StatesUpdated();
    }

    private void StatesUpdated()
    {
        switch (_settings.Output)
        {
            case DeviceOut.Blend:
                RefreshBlend(_outputLevelCache.MonitorBlend);
                return;
            case DeviceOut.Phones:
                RefreshVolume(_outputLevelCache.HeadphonesVolume);
                return;
            case DeviceOut.MainOut:
                RefreshVolume(_outputLevelCache.MainOutVolume);
                return;
        }
    }

    private void RefreshBlend(float valueRaw)
    {
        var blend = OutputRawToBlend(valueRaw);

        if (_isEncoder)
        {
            var percentage = valueRaw * 100f;

            Connection.SetFeedbackAsync(JObject.FromObject(new
            {
                value = $"{blend:0.00}",
                indicator = percentage
            }));
        }
        else
        {
            Connection.SetTitleAsync($"{blend:0.00}");
        }
    }

    private void RefreshVolume(float valueRaw)
    {
        var volumeDb = LookupTable.OutputPercentageToDb(valueRaw);
        if (_isEncoder)
        {
            var indicatorPercentage = valueRaw * 100f;

            Connection.SetFeedbackAsync(JObject.FromObject(new
            {
                value = $"{volumeDb:0.00} dB",
                indicator = indicatorPercentage
            }));
        }
        else
        {
            Connection.SetTitleAsync($"{volumeDb:0.00} dB");
        }
    }

    private void RefreshState()
    {
        switch (_settings.Output)
        {
            case DeviceOut.Blend:
            case DeviceOut.Phones:
                Connection.SetStateAsync(0);
                return;
            default:
                Connection.SetStateAsync(_device.Main.HardwareMute ? 1u : 0u);
                return;
        }
    }

    #region NotUsed

    public override void DialDown(DialPayload payload)
    {
        // We don't have an action here.
    }

    public override void DialUp(DialPayload payload)
    {
        //
    }

    public override void TouchPress(TouchpadPressPayload payload)
    {
        //
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
        //
    }

    public override void OnTick()
    {
        //
    }

    #endregion
}
