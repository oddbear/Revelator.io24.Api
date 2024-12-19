using System.Diagnostics;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using BarRaider.SdTools;
using BarRaider.SdTools.Events;
using BarRaider.SdTools.Wrappers;

namespace Revelator.io24.StreamDeck.Actions
{
    [PluginActionId("com.oddbear.revelator.io24.volumelevel")]
    public class VolumeLevelAction : KeypadBase
    {
        private readonly VolumeLevelSettings _settings;

        private readonly RoutingTable _routingTable;

        public VolumeLevelAction(
            ISDConnection connection,
            InitialPayload payload)
                : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                _settings = new VolumeLevelSettings();
            }
            else
            {
                _settings = payload.Settings.ToObject<VolumeLevelSettings>()!;
            }

            _routingTable = Program.RoutingTable;

            Connection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
        }

        private void Connection_OnPropertyInspectorDidAppear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidAppear> e)
        {
            _routingTable.VolumeUpdated += VolumeUpdated;
        }

        private void Connection_OnPropertyInspectorDidDisappear(object? sender, SDEventReceivedEventArgs<PropertyInspectorDidDisappear> e)
        {
            _routingTable.VolumeUpdated -= VolumeUpdated;
        }
        
        //protected override bool GetButtonState()
        //{
        //    //There is only one state:
        //    return true;
        //}
        
        private async void VolumeUpdated(object? sender, (Input input, MixOut output) e)
        {
            try
            {
                var route = (_settings.Input, Output: _settings.MixOut);
                if (e != route)
                    return;

                var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
                await Connection.SetTitleAsync($"{value} dB");
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        public override void KeyPressed(KeyPayload payload)
        {
            var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
            switch (_settings.ChangeType)
            {
                case VolumeType.Increment:
                    value += _settings.Value;
                    _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                    break;
                case VolumeType.Decrement:
                    value -= _settings.Value;
                    _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                    break;
                case VolumeType.Absolute:
                default:
                    value = _settings.Value;
                    _routingTable.SetVolumeInDb(_settings.Input, _settings.MixOut, value);
                    break;
            }
        }

        public override void KeyReleased(KeyPayload payload)
        {
            //
        }

        public async override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            var value = _routingTable.GetVolumeInDb(_settings.Input, _settings.MixOut);
            await Connection.SetTitleAsync($"{value} dB");
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
            //
        }

        public override void OnTick()
        {
            //
        }

        public override void Dispose()
        {
            //
        }
    }
}
