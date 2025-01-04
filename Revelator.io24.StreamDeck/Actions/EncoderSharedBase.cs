using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using Revelator.io24.Api;
using System.Globalization;

namespace Revelator.io24.StreamDeck.Actions;

public abstract class EncoderSharedBase<TSettings> : EncoderBase
    where TSettings : class, new()
{
    protected TSettings _settings { get; set; }

    protected readonly RoutingTable _routingTable;
    protected readonly Device _device;

    protected EncoderSharedBase(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _device = Program.Device;
        _routingTable = Program.RoutingTable;

        _settings ??= new TSettings();
    }
    
    public override void DialUp(DialPayload payload)
    {
        // We only react on key press.
    }

    public override void TouchPress(TouchpadPressPayload payload)
    {
        // We only react on key press.
    }

    public override void OnTick()
    {
        // Used to update UI each second, we don't need this as we know when UI values change.
    }

    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
    {
        // We don't have global settings.
    }

    protected async Task SetFeedbackAsync(FeedbackCard feedbackCard)
    {
        var dkv = new Dictionary<string, string>
        {
            // Volume Title in percentage:
            ["value"] = feedbackCard.Value,
            // Volume bar in percentage 0-100:
            ["indicator"] = feedbackCard.Indicator.ToString(CultureInfo.InvariantCulture)
        };

        await Connection.SetFeedbackAsync(dkv);
    }
}

public class FeedbackCard
{
    public required string Value { get; init; }
    public required float Indicator { get; init; }
}