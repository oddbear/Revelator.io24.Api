using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly UpdateService _updateService;

        public RevelatorIo24Plugin(ITouchPortalClientFactory clientFactory,
            UpdateService updateService)
        {
            //Set the event handler for TouchPortal:
            _client = clientFactory.Create(this);

            _updateService = updateService;
            _updateService.RoutingUpdated += RoutingUpdated;
        }

        private void RoutingUpdated(object? sender, EventArgs e)
        {
            var routing = _updateService.Routing;

            _client.StateUpdate(PluginId + ".states.headphonessource", routing.HeadphonesSource.ToString());

            _client.StateUpdate(PluginId + ".states.line/ch1/mute", routing.Main_MicL ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.line/ch2/mute", routing.Main_MicR ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.return/ch1/mute", routing.Main_Playback ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.return/ch2/mute", routing.Main_VirtualA ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.return/ch3/mute", routing.Main_VirtualB ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.main/ch1/mute", routing.Main_Mix ? "On" : "Off");

            _client.StateUpdate(PluginId + ".states.line/ch1/assign_aux1", routing.MixA_MicL ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.line/ch2/assign_aux1", routing.MixA_MicR ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.return/ch1/assign_aux1", routing.MixA_Playback ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.return/ch2/assign_aux1", routing.MixA_VirtualA ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.return/ch3/assign_aux1", routing.MixA_VirtualB ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.aux/ch1/mute", routing.MixA_Mix ? "On" : "Off");

            _client.StateUpdate(PluginId + ".states.line/ch1/assign_aux2", routing.MixB_MicL ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.line/ch2/assign_aux2", routing.MixB_MicR ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.return/ch1/assign_aux2", routing.MixB_Playback ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.return/ch2/assign_aux2", routing.MixB_VirtualA ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.return/ch3/assign_aux2", routing.MixB_VirtualB ? "On" : "Off");
            _client.StateUpdate(PluginId + ".states.aux/ch2/mute", routing.MixB_Mix ? "On" : "Off");
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

            _updateService.SetRouteValue(route, value);
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

            var hasRoute = _updateService.Routing.GetValueByRoute(route);
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
                    _updateService.SetRouteValue("global/phonesSrc", 0.0f);
                    break;
                case Headphones.MixA:
                    _updateService.SetRouteValue("global/phonesSrc", 0.5f);
                    break;
                case Headphones.MixB:
                    _updateService.SetRouteValue("global/phonesSrc", 1.0f);
                    break;
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
            //Ignore for now, can implement when I have volume set in the API.
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
