using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Extensions;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
using Revelator.io24.TouchPortal.Converters;
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
        private readonly Microphones _microphones;

        public RevelatorIo24Plugin(
            ITouchPortalClientFactory clientFactory,
            RoutingTable routingTable,
            Microphones microphones)
        {
            //Set the event handler for TouchPortal:
            _client = clientFactory.Create(this);

            _routingTable = routingTable;
            _microphones = microphones;

            _routingTable.HeadphoneUpdated += HeadphoneUpdated;
            _routingTable.RouteUpdated += RouteUpdated;
            _routingTable.VolumeUpdated += VolumeUpdated;

            _microphones.FatChannelUpdated += FatChannelUpdated;
            _microphones.PresetsUpdated += (s, e) => { };
            _microphones.PresetUpdated += PresetUpdated;
        }

        private void VolumeUpdated(object? sender, (Input input, Output output) e)
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

        private void HeadphoneUpdated(object? sender, Headphones e)
        {
            var description = e.GetDescription();
            _client.StateUpdate(PluginId + ".states.headphonessource", description);
        }

        private void RouteUpdated(object? sender, (Input input, Output output) e)
        {
            var value = _routingTable.GetRouting(e.input, e.output);
            _client.StateUpdate($"{PluginId}.states.{e.input}|{e.output}", value ? "On" : "Off");
        }

        private void FatChannelUpdated(object? sender, MicrophoneChannel e)
        {
            var value = _microphones.GetFatChannelStatus(e);
            _client.StateUpdate($"{PluginId}.states.fatchannel.{e}", value ? "On" : "Off");
        }

        private void PresetUpdated(object? sender, MicrophoneChannel e)
        {
            var value = _microphones.GetPreset(e);
            _client.StateUpdate($"{PluginId}.states.preset.{e}", value);
        }

        private void RoutingUpdated(object? sender, string route)
        {
            //var headphones = _routingModel.RouteValues["global/phonesSrc"];
            //switch (headphones)
            //{
            //    case 0.0f:
            //        _client.StateUpdate(PluginId + ".states.headphonessource", "Main");
            //        break;
            //    case 0.5f:
            //        _client.StateUpdate(PluginId + ".states.headphonessource", "Mix A");
            //        break;
            //    case 1.0f:
            //        _client.StateUpdate(PluginId + ".states.headphonessource", "Mix B");
            //        break;
            //}

            //UpdateState("line/ch1/mute");
            //UpdateState("line/ch2/mute");
            //UpdateState("return/ch1/mute");
            //UpdateState("return/ch2/mute");
            //UpdateState("return/ch3/mute");
            //UpdateState("main/ch1/mute");

            //UpdateState("line/ch1/assign_aux1");
            //UpdateState("line/ch2/assign_aux1");
            //UpdateState("return/ch1/assign_aux1");
            //UpdateState("return/ch2/assign_aux1");
            //UpdateState("return/ch3/assign_aux1");
            //UpdateState("aux/ch1/mute");

            //UpdateState("line/ch1/assign_aux2");
            //UpdateState("line/ch2/assign_aux2");
            //UpdateState("return/ch1/assign_aux2");
            //UpdateState("return/ch2/assign_aux2");
            //UpdateState("return/ch3/assign_aux2");
            //UpdateState("aux/ch2/mute");

            //UpdateState("line/ch1/bypassDSP");
            //UpdateState("line/ch2/bypassDSP");

            //if (route == "synchronize")
            //{
            //    foreach (var key in _routingModel.VolumeValue.Keys)
            //    {
            //        UpdateConnector(key);
            //    }
            //}
            //else
            //{
            //    UpdateConnector(route);
            //}
        }

        private void UpdateState(string route)
        {
            //var hasRoute = _routingModel.GetRouteBooleanState(route);
            //if (route.EndsWith("mute") || route.EndsWith("/bypassDSP"))
            //    hasRoute = !hasRoute;

            //_client.StateUpdate(PluginId + ".states." + route, hasRoute ? "On" : "Off");
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

            _routingTable.SetHeadphoneSource(headphone);
        }

        private void FatChannelChange(string name, IReadOnlyCollection<ActionDataSelected> datalist)
        {
            var dict = datalist
                .ToDictionary(kv => kv.Id, kv => kv.Value);

            var microphone = dict[name + ".microphone"];
            var actions = dict[name + ".actions"];

            var channel = MicrophoneChannelConverter.GetMicrophoneChannel(microphone);
            var value = ValueConverter.GetValue(actions);

            _microphones.SetFatChannelStatus(channel, value);
        }

        private void PresetChange(string name, IReadOnlyCollection<ActionDataSelected> datalist)
        {
            var dict = datalist
                .ToDictionary(kv => kv.Id, kv => kv.Value);

            var microphone = dict[name + ".microphone"];
            var action = dict[name + ".preset"];

            var channel = MicrophoneChannelConverter.GetMicrophoneChannel(microphone);

            _microphones.SetPreset(channel, action);
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
                var output = EnumExtensions.ParseDescription<Output>(outputDesc);

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
                var presets = _microphones.GetPresets(microphone);
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
