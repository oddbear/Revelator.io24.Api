using System.ComponentModel;
using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using Revelator.io24.Api.Enums;
using Newtonsoft.Json.Linq;
using Revelator.io24.Api;
using Revelator.io24.Api.Models;
using Revelator.io24.StreamDeck.Actions.Enums;
using Revelator.io24.StreamDeck.Actions.Encoders.OutputLevel;

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
    }

    private void CachedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _ = RefreshVolume();
    }

    private void SkipCacheRouteUpdated(object? sender, (Input, MixOut) e)
    {
        // TODO: Is this good and fast enough?
        _ = RefreshState();
    }

    public override async Task KeyPressedAsync(KeyPayload payload)
    {
        if (_settings is not {
            Input: Input input,
            MixOut: MixOut mixOut,
            Action: VolumeActionType action
        })
        {
            await Connection.ShowAlert();
            return;
        }

        switch (action)
        {
            case VolumeActionType.Mute:
                _routingTable.SetRouting(input, mixOut, _settings.RouteValue);
                break;
            case VolumeActionType.Set:
                _volumeLevelCache.SetVolume(input, mixOut, new VolumeValue { ValueDb = _settings.SetVolume });
                break;
            default:
                var value = _volumeLevelCache.GetVolume(input, mixOut);
                value.ValueDb += _settings.AdjustVolume;

                if (value.ValueDb < -96)
                    value.ValueDb = -96;

                if (value.ValueDb > 10)
                    value.ValueDb = 10;

                _volumeLevelCache.SetVolume(input, mixOut, value);
                break;
        }
    }

    public override async Task KeyReleasedAsync(KeyPayload payload)
    {
        await RefreshState();
    }

    public override async Task DialRotateAsync(DialRotatePayload payload)
    {
        if (_settings is not
        {
            Input: Input input,
            MixOut: MixOut mixOut
        })
        {
            await Connection.ShowAlert();
            return;
        }

        var volume = _volumeLevelCache.GetVolume(input, mixOut);

        var adjustment = 0.01f * payload.Ticks;
        volume.ValueRaw += adjustment;

        if (volume.ValueRaw < 0)
            volume.ValueRaw = 0;

        if (volume.ValueRaw > 1)
            volume.ValueRaw = 1;

        // VolumeCache will trigger global PropertyChanged event.
        _volumeLevelCache.SetVolume(input, mixOut, volume);
    }

    public override async Task DialDownAsync(DialPayload payload)
    {
        if (_settings is not
        {
            Input: Input input,
            MixOut: MixOut mixOut
        })
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
        if (_settings is not
            {
                Input: Input input,
                MixOut: MixOut mixOut
            })
        {
            return;
        }

        // If Keypad press, cache should be empty.
        var value = _volumeLevelCache.GetVolume(input, mixOut);
        if (_isEncoder)
        {
            await Connection.SetFeedbackAsync(JObject.FromObject(new
            {
                value = $"{value.ValueDb:0.00} dB",
                indicator = value.ValuePercent
            }));
        }
        else
        {
            await Connection.SetTitleAsync($"{value.ValueDb:0.00} dB");
        }
    }

    private async Task RefreshState()
    {
        if (_isEncoder)
            return;

        if (_settings is not
            {
                Input: Input input,
                MixOut: MixOut mixOut
            })
        {
            return;
        }

        var routing = _routingTable.GetRouting(input, mixOut);
        await Connection.SetStateAsync(routing ? 0u : 1u);
    }
}
