using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Extensions;
using Revelator.io24.Api.Models.Global;
using Revelator.io24.Api.Models.Inputs;
using Revelator.io24.TouchPortal.Converters;
using System.ComponentModel;
using System.Text.Json;
using TouchPortalSDK;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Messages.Models;

namespace Revelator.io24.TouchPortal
{
    public class RevelatorIo24Plugin : ITouchPortalEventHandler
    {
        public string PluginId => "oddbear.touchportal.revelator.io24";

        private readonly ITouchPortalClient _client;
        private readonly RoutingTable _routingTable;
        private readonly Device _device;

        public RevelatorIo24Plugin(
            ITouchPortalClientFactory clientFactory,
            RoutingTable routingTable,
            Device device)
        {
            //Set the event handler for TouchPortal:
            _client = clientFactory.Create(this);

            _routingTable = routingTable;
            _device = device;

            _routingTable.RouteUpdated += RouteUpdated;
            _routingTable.VolumeUpdated += VolumeUpdated;

            _device.Global.PropertyChanged += Global_PropertyChanged;
            _device.MicrohoneLeft.PropertyChanged += Microhone_PropertyChanged;
            _device.MicrohoneRight.PropertyChanged += Microhone_PropertyChanged;
        }

        private void Global_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not Global global)
                return;

            switch (e.PropertyName)
            {
                case nameof(Global.HeadphonesSource):
                    var description = global.HeadphonesSource.GetDescription();
                    _client.StateUpdate(PluginId + ".states.headphonessource", description);
                    return;
            }
        }

        private void Microhone_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not LineChannel channel)
                return;

            var endpoint = sender is MicrohoneLeft
                ? MicrophoneChannel.Left
                : MicrophoneChannel.Right;

            switch (e.PropertyName)
            {
                case nameof(LineChannel.BypassDSP):
                    _client.StateUpdate($"{PluginId}.states.fatchannel.{endpoint}", channel.BypassDSP ? "Off" : "On");
                    return;

                case nameof(LineChannel.Preset):
                    _client.StateUpdate($"{PluginId}.states.preset.{endpoint}", channel.Preset);
                    return;
            }
        }

        private void VolumeUpdated(object? sender, (Input input, MixOut output) e)
        {
            var value = _routingTable.GetVolume(e.input, e.output);

            var inputDesc = e.input.GetDescription();
            var outputDesc = e.output.GetDescription();

            var message = JsonSerializer.Serialize(new
            {
                type = "connectorUpdate",
                connectorId = $"pc_{PluginId}_tp_io24_volume|tp_io24_volume_input={inputDesc}|tp_io24_volume_output={outputDesc}",
                value
            });

            _client.SendMessage(message);
        }

        private void RouteUpdated(object? sender, (Input input, MixOut output) e)
        {
            var value = _routingTable.GetRouting(e.input, e.output);
            _client.StateUpdate($"{PluginId}.states.{e.input}|{e.output}", value ? "On" : "Off");
        }

        public void Init()
        {
            //Connecting to TouchPortal:
            _client.Connect();
        }

        public void OnActionEvent(ActionEvent message)
        {
            var actionId = message.ActionId;

            if (actionId.EndsWith(".routingtable.action.change"))
            {
                RouteChange(actionId + ".data", message.Data);
            }

            if (actionId.EndsWith(".headphones.action.change"))
            {
                HeadphonesSourceChange(actionId + ".data", message.Data);
            }

            if (actionId.EndsWith(".fatchanneltoggle.action.change"))
            {
                FatChannelChange(actionId + ".data", message.Data);
            }

            if (actionId.EndsWith(".presetchange.action.change"))
            {
                PresetChange(actionId + ".data", message.Data);
            }
        }

        private void RouteChange(string name, IReadOnlyCollection<ActionDataSelected> datalist)
        {
            var dict = datalist
                .ToDictionary(kv => kv.Id, kv => kv.Value);

            var inputs = dict[name + ".inputs"];
            var outputs = dict[name + ".outputs"];
            var actions = dict[name + ".actions"];

            var input = InputConverter.GetInput(inputs);
            var output = OutputConverter.GetOutput(outputs);
            var action = ValueConverter.GetValue(actions);

            _routingTable.SetRouting(input, output, action);
        }

        private void HeadphonesSourceChange(string name, IReadOnlyCollection<ActionDataSelected> datalist)
        {
            var dict = datalist
                .ToDictionary(kv => kv.Id, kv => kv.Value);

            var headphoneValue = dict[name + ".headphones"];
            var headphone = GetHeadphoneEnum(headphoneValue);

            _device.Global.HeadphonesSource = headphone;
        }

        private void FatChannelChange(string name, IReadOnlyCollection<ActionDataSelected> datalist)
        {
            var dict = datalist
                .ToDictionary(kv => kv.Id, kv => kv.Value);

            var microphone = dict[name + ".microphone"];
            var actions = dict[name + ".actions"];

            var channel = MicrophoneChannelConverter.GetMicrophoneChannel(microphone);
            var value = ValueConverter.GetValue(actions);

            var lineChannel = channel == MicrophoneChannel.Left
                ? _device.MicrohoneLeft as LineChannel
                : _device.MicrohoneRight as LineChannel;

            switch (value)
            {
                case Value.On:
                    lineChannel.BypassDSP = false;
                    return;
                case Value.Off:
                    lineChannel.BypassDSP = true;
                    return;
                case Value.Toggle:
                default:
                    lineChannel.BypassDSP = !lineChannel.BypassDSP;
                    return;
            }
        }

        private void PresetChange(string name, IReadOnlyCollection<ActionDataSelected> datalist)
        {
            var dict = datalist
                .ToDictionary(kv => kv.Id, kv => kv.Value);

            var microphone = dict[name + ".microphone"];
            var preset = dict[name + ".preset"];

            var channel = MicrophoneChannelConverter.GetMicrophoneChannel(microphone);
            if (channel == MicrophoneChannel.Left)
            {
                _device.MicrohoneLeft.Preset = preset;
            }
            else
            {
                _device.MicrohoneRight.Preset = preset;
            }
        }

        private Headphones GetHeadphoneEnum(string headphoneValue)
        {
            return headphoneValue switch
            {
                "Main" => Headphones.Main,
                "Stream Mix A" => Headphones.MixA,
                "Stream Mix B" => Headphones.MixB,
                _ => throw new InvalidOperationException(),
            };
        }

        public void OnClosedEvent(string message)
        {
            Environment.Exit(0);
        }

        public void OnBroadcastEvent(BroadcastEvent message)
        {
            //Ignore for now, should be used to refresh values.
        }

        public void OnConnecterChangeEvent(ConnectorChangeEvent message)
        {
            if(message.ConnectorId == "tp_io24_volume")
            {
                var inputDesc = message.Data.Single(d => d.Id == "tp_io24_volume_input").Value;
                var outputDesc = message.Data.Single(d => d.Id == "tp_io24_volume_output").Value;
                var input = EnumExtensions.ParseDescription<Input>(inputDesc);
                var output = EnumExtensions.ParseDescription<MixOut>(outputDesc);

                _routingTable.SetVolume(input, output, message.Value);
            }
        }

        public void OnInfoEvent(InfoEvent message)
        {
            //Set states?
        }

        public void OnListChangedEvent(ListChangeEvent message)
        {
            if (message.ActionId.EndsWith(".presetchange.action.change")
             && message.ListId.EndsWith(".microphone"))
            {
                var microphone = MicrophoneChannelConverter.GetMicrophoneChannel(message.Value);
                var presets = microphone == MicrophoneChannel.Left
                    ? _device.MicrohoneLeft.Presets
                    : _device.MicrohoneRight.Presets;

                _client.ChoiceUpdate(PluginId + ".presetchange.action.change.data.preset", presets, message.InstanceId);
            }
        }

        public void OnNotificationOptionClickedEvent(NotificationOptionClickedEvent message)
        {
            //Ignore for now.
        }

        public void OnSettingsEvent(SettingsEvent message)
        {
            //Ignore for now.
        }

        public void OnUnhandledEvent(string jsonMessage)
        {
            //Ignore for now.
        }
    }
}
