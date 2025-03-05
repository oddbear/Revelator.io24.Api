using BarRaider.SdTools;
using BarRaider.SdTools.Payloads;
using Revelator.io24.Api;
using Revelator.io24.StreamDeck.Actions.Enums;

namespace Revelator.io24.StreamDeck.Actions;

public abstract class EncoderSharedBase<TSettings> : EncoderBase, IKeypadPlugin
    where TSettings : class, new()
{
    protected TSettings _settings { get; set; }

    private readonly Controller _controller;

    protected readonly RoutingTable _routingTable;
    protected readonly Device _device;

    protected EncoderSharedBase(
        ISDConnection connection,
        InitialPayload payload)
        : base(connection, payload)
    {
        _controller = Enum.Parse<Controller>(payload.Controller);

        _device = Program.Device;
        _routingTable = Program.RoutingTable;

        _settings ??= new TSettings();
    }

    protected abstract Task RefreshState();

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
    
    public abstract void KeyPressed(KeyPayload payload);

    public void KeyReleased(KeyPayload payload)
    {
        // We only react on key press, but we need to refresh the state because of the automatic UI change by Elgato.
        RefreshState();
    }
}
