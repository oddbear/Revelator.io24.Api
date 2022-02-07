using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Extensions;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;
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
        private readonly CommunicationService _communicationService;
        private readonly RoutingModel _routingModel;

        public RevelatorIo24Plugin(ITouchPortalClientFactory clientFactory,
            CommunicationService communicationService,
            RoutingModel routingModel)
        {
            //Set the event handler for TouchPortal:
            _client = clientFactory.Create(this);

            _communicationService = communicationService;
            _routingModel = routingModel;

            routingModel.RoutingUpdated += RoutingUpdated;
        }

        private void RoutingUpdated(object? sender, string route)
        {
            var headphones = _routingModel.RouteValues["global/phonesSrc"];
            switch (headphones)
            {
                case 0.0f:
                    _client.StateUpdate(PluginId + ".states.headphonessource", "Main");
                    break;
                case 0.5f:
                    _client.StateUpdate(PluginId + ".states.headphonessource", "Mix A");
                    break;
                case 1.0f:
                    _client.StateUpdate(PluginId + ".states.headphonessource", "Mix B");
                    break;
            }

            UpdateState("line/ch1/mute");
            UpdateState("line/ch2/mute");
            UpdateState("return/ch1/mute");
            UpdateState("return/ch2/mute");
            UpdateState("return/ch3/mute");
            UpdateState("main/ch1/mute");

            UpdateState("line/ch1/assign_aux1");
            UpdateState("line/ch2/assign_aux1");
            UpdateState("return/ch1/assign_aux1");
            UpdateState("return/ch2/assign_aux1");
            UpdateState("return/ch3/assign_aux1");
            UpdateState("aux/ch1/mute");

            UpdateState("line/ch1/assign_aux2");
            UpdateState("line/ch2/assign_aux2");
            UpdateState("return/ch1/assign_aux2");
            UpdateState("return/ch2/assign_aux2");
            UpdateState("return/ch3/assign_aux2");
            UpdateState("aux/ch2/mute");

            UpdateState("line/ch1/bypassDSP");
            UpdateState("line/ch2/bypassDSP");

            if (route == "synchronize")
            {
                foreach (var key in _routingModel.VolumeValues.Keys)
                {
                    UpdateConnector(key);
                }
            }
            else
            {
                UpdateConnector(route);
            }
        }

        private void UpdateConnector(string route)
        {
            var volume = _routingModel.VolumeValues[route];
            var value = (int)Math.Round(volume * 100f);

            var (input, output) = _routingModel.GetVolumeInputOutput(route);
            
            var inputDesc = input.GetDescription();
            var outputDesc = output.GetDescription();

            var message = JsonSerializer.Serialize(new
            {
                type = "connectorUpdate",
                connectorId = $"pc_{PluginId}_tp_io24_volume|tp_io24_volume_input={inputDesc}|tp_io24_volume_output={outputDesc}",
                value
            });

            _client.SendMessage(message);
        }

        private void UpdateState(string route)
        {
            var hasRoute = _routingModel.GetRouteBooleanState(route);
            if (route.EndsWith("mute") || route.EndsWith("/bypassDSP"))
                hasRoute = !hasRoute;

            _client.StateUpdate(PluginId + ".states." + route, hasRoute ? "On" : "Off");
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
        }

        private void RouteChange(string name, IReadOnlyCollection<ActionDataSelected> datalist)
        {
            var dict = datalist
                .ToDictionary(kv => kv.Id, kv => kv.Value);

            var inputs = dict[name + ".inputs"];
            var outputs = dict[name + ".outputs"];
            var actions = dict[name + ".actions"];

            var input = InputToPart(inputs);
            var output = OutputToPart(outputs);

            var route = $"{input}/{output}";
            var value = ActionToValue(route, actions);

            _communicationService.SetRouteValue(route, value);
        }

        private float ActionToValue(string route, string action)
        {
            if (action == "Turn On")
            {
                return route.EndsWith("mute")
                    ? 0.0f
                    : 1.0f;
            }

            if (action == "Turn Off")
            {
                return route.EndsWith("mute")
                    ? 1.0f
                    : 0.0f;
            }

            var hasRoute = _routingModel.GetRouteBooleanState(route);
            return route.EndsWith("mute")
                ? (hasRoute ? 1.0f : 0.0f)
                : (hasRoute ? 0.0f : 1.0f);
        }

        private string InputToPart(string input)
        {
            return input switch
            {
                "Mic L" => "line/ch1",
                "Mic R" => "line/ch2",
                "Playback" => "return/ch1",
                "Virual A" => "return/ch2",
                "Virual B" => "return/ch3",
                "Mute All" => "main/ch1",
                _ => throw new InvalidOperationException(),
            };
        }

        private string OutputToPart(string output)
        {
            switch (output)
            {
                case "Main":
                    return "mute";
                case "Stream Mix A":
                    return "assign_aux1";
                case "Stream Mix B":
                    return "assign_aux2";
                default:
                    throw new InvalidOperationException();
            }
        }

        private void HeadphonesSourceChange(string name, IReadOnlyCollection<ActionDataSelected> datalist)
        {
            var dict = datalist
                .ToDictionary(kv => kv.Id, kv => kv.Value);

            var headphoneValue = dict[name + ".headphones"];
            var headphone = GetHeadphoneEnum(headphoneValue);

            switch (headphone)
            {
                case Headphones.Main:
                    _communicationService.SetRouteValue("global/phonesSrc", 0.0f);
                    break;
                case Headphones.MixA:
                    _communicationService.SetRouteValue("global/phonesSrc", 0.5f);
                    break;
                case Headphones.MixB:
                    _communicationService.SetRouteValue("global/phonesSrc", 1.0f);
                    break;
            }
        }

        private void FatChannelChange(string name, IReadOnlyCollection<ActionDataSelected> datalist)
        {
            var dict = datalist
                .ToDictionary(kv => kv.Id, kv => kv.Value);

            var microphone = dict[name + ".microphone"];
            var action = dict[name + ".actions"];

            var route = GetFatChannelRoute(microphone);
            var state = GetCurrentFatChannelState(route);
            var value = GetFatChannelAction(action, route);

            _communicationService.SetRouteValue(route, value);
        }

        private string GetFatChannelRoute(string microphone)
            => microphone switch
            {
                "Mic L" => "line/ch1/bypassDSP",
                "Mic R" => "line/ch2/bypassDSP",
                _ => throw new InvalidOperationException()
            };

        private bool GetCurrentFatChannelState(string route)
            => _routingModel.GetRouteBooleanState(route);

        private float GetFatChannelAction(string action, string route)
            => action switch
            {
                "Turn On" => 0.0f,
                "Turn Off" => 1.0f,
                _ => GetCurrentFatChannelState(route) ? 0.0f : 1.0f,
            };

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

                var route = _routingModel.GetVolumeRoute(input, output);

                var value = message.Value / 100.0f;
                _communicationService.SetRouteValue(route, value);
            }
        }

        public void OnInfoEvent(InfoEvent message)
        {
            //Set states?
        }

        public void OnListChangedEvent(ListChangeEvent message)
        {
            //Ignore for now.
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
