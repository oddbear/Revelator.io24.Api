using System.ComponentModel;
using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;
using Revelator.io24.Api;
using Revelator.io24.Api.Models.ValueConverters;
using Revelator.io24.StreamDeck.Actions.Enums;

namespace Revelator.io24.StreamDeck.Actions.Encoders.VolumeLevel;

[PluginActionId("com.oddbear.revelator.io24.encoder.volume-level")]
public class VolumeLevelEncoder : HybridBase
{
    private readonly VolumeLevelCache _volumeLevelCache;
    private readonly RoutingTable _routingTable;
    private readonly bool _isEncoder;

    private VolumeLevelEncoderSettings _settings;

    public VolumeLevelEncoder(ISDConnection connection, InitialPayload payload)
        : base(connection, payload)
    {
        _isEncoder = payload.Controller == "Encoder";

        _volumeLevelCache = Program.VolumeLevelCache;
        _routingTable = Program.RoutingTable;

        if (payload.Settings == null || payload.Settings.Count == 0)
        {
            _settings = new VolumeLevelEncoderSettings();
        }
        else
        {
            _settings = payload.Settings.ToObject<VolumeLevelEncoderSettings>()!;
            _ = RefreshVolume();
            _ = RefreshState();
        }

        _volumeLevelCache.PropertyChanged += CachedPropertyChanged;
        _routingTable.RouteUpdated += SkipCacheRouteUpdated;
    }

    public override void Dispose()
    {
        _volumeLevelCache.PropertyChanged -= CachedPropertyChanged;
        _routingTable.RouteUpdated -= SkipCacheRouteUpdated;
    }

    private void CachedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _ = RefreshVolume();
    }

    private void SkipCacheRouteUpdated(object? sender, (Input, MixOut) e)
    {
        _ = RefreshState();
    }

    public override void TouchPress(TouchpadPressPayload payload)
    {
        if (!ValidEncoder(out var input, out var mixOut))
        {
            Connection.ShowAlert();
            return;
        }

        _routingTable.SetRouting(input, mixOut, Value.Toggle);
    }

    public override async Task KeyPressedAsync(KeyPayload payload)
    {
        if (!ValidKeyPad(out var input, out var mixOut, out var action))
        {
            await Connection.ShowAlert();
            return;
        }

        switch (action)
        {
            case VolumeActionType.Mute:
                _routingTable.SetRouting(input, mixOut, _settings.RouteValue);
                break;
            case VolumeActionType.Solo:
                _routingTable.SetSoloMono(input, mixOut, _settings.RouteValue);
                break;
            case VolumeActionType.Set:
                _volumeLevelCache.SetVolume(input, mixOut, new VolumeValue { Db = _settings.SetVolume });
                break;
            default:
                var volume = _volumeLevelCache.GetVolume(input, mixOut);
                volume.Db += _settings.AdjustVolume;
                _volumeLevelCache.SetVolume(input, mixOut, volume);
                break;
        }
    }

    public override async Task KeyReleasedAsync(KeyPayload payload)
    {
        await RefreshState();
    }

    public override async Task DialRotateAsync(DialRotatePayload payload)
    {
        if (!ValidEncoder(out var input, out var mixOut))
        {
            await Connection.ShowAlert();
            return;
        }

        // VolumeCache will trigger global PropertyChanged event.
        var volume = _volumeLevelCache.GetVolume(input, mixOut);
        volume.Percent += payload.Ticks;
        _volumeLevelCache.SetVolume(input, mixOut, volume);
    }

    public override async Task DialDownAsync(DialPayload payload)
    {
        if (!ValidEncoder(out var input, out var mixOut))
        {
            await Connection.ShowAlert();
            return;
        }

        _routingTable.SetRouting(input, mixOut, Value.Toggle);
    }

    public override async Task ReceivedSettingsAsync(ReceivedSettingsPayload payload)
    {
        Tools.AutoPopulateSettings(_settings, payload.Settings);
        await RefreshVolume();
        await RefreshState();
    }

    private async Task RefreshVolume()
    {
        if (!ValidEncoder(out var input, out var mixOut))
            return;

        // If Keypad press, cache should be empty.
        var value = _volumeLevelCache.GetVolume(input, mixOut);
        if (_isEncoder)
        {
            await Connection.SetFeedbackAsync(JObject.FromObject(new
            {
                value = $"{value.Db:0.00} dB",
                indicator = value.Percent,
                title = input.ToString().Replace("_"," ")
            }));
        }
        else
        {
            await Connection.SetTitleAsync($"{value.Db:0.00} dB");
        }
    }

    private async Task RefreshState()
    {
        // We don't need action here:
        if (!ValidEncoder(out var input, out var mixOut))
            return;

        bool routing = _routingTable.GetRouting(input, mixOut);
        if (_isEncoder)
        {
            var iconImage = routing ? "Images/Plugin/volume_on" : "Images/Plugin/volume_off";
            await Connection.SetFeedbackAsync("icon", iconImage);
            return;
        }

        switch (_settings.Action)
        {
            case VolumeActionType.Mute:
                await Connection.SetStateAsync(routing ? 0u : 1u);
                return;
            case VolumeActionType.Solo:
                var solo = _routingTable.GetSoloMono(input, mixOut);
                await Connection.SetStateAsync(solo ? 0u : 1u);
                return;
        }

    }

    private bool ValidKeyPad(out Input input, out MixOut mixOut, out VolumeActionType action)
    {
        input = _settings.Input ?? default;
        mixOut = _settings.MixOut ?? default;
        action = _settings.Action ?? default;

        return _settings is
        {
            Input: not null,
            MixOut: not null,
            Action: not null
        };
    }

    private bool ValidEncoder(out Input input, out MixOut mixOut)
    {
        input = _settings.Input ?? default;
        mixOut = _settings.MixOut ?? default;

        return _settings is
        {
            Input: not null,
            MixOut: not null
        };
    }
}
