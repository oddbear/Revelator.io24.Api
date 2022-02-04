// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
    uuid = null,
    inInfo = null,
    actionInfo = {},
    settingsModel = {
        Route: "line/ch1/bypassDSP",
        Action: "Toggle"
    };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
    uuid = inUUID;
    actionInfo = JSON.parse(inActionInfo);
    inInfo = JSON.parse(inInfo);
    websocket = new WebSocket('ws://localhost:' + inPort);

    //initialize values
    if (actionInfo.payload.settings.settingsModel) {
        settingsModel.Route = actionInfo.payload.settings.settingsModel.Route;
        settingsModel.Action = actionInfo.payload.settings.settingsModel.Action;
    }

    document.getElementById('routeValue').value = settingsModel.Route;
    document.getElementById('actionValue').value = settingsModel.Action;
    
    websocket.onopen = function () {
        var json = { event: inRegisterEvent, uuid: inUUID };
        // register property inspector to Stream Deck
        websocket.send(JSON.stringify(json));
    };

    websocket.onmessage = function (evt) {
        // Received message from Stream Deck
        var jsonObj = JSON.parse(evt.data);
        var sdEvent = jsonObj['event'];
        switch (sdEvent) {
            case "didReceiveSettings":
                if (jsonObj.payload.settings.settingsModel.Route) {
                    settingsModel.Route = jsonObj.payload.settings.settingsModel.Route;
                    document.getElementById('routeValue').value = settingsModel.Route;
                }
                if (jsonObj.payload.settings.settingsModel.Action) {
                    settingsModel.Action = jsonObj.payload.settings.settingsModel.Action;
                    document.getElementById('actionValue').value = settingsModel.Action;
                }
                break;
            default:
                break;
        }
    };
}

const setSettings = (value, param) => {
    if (websocket) {
        settingsModel[param] = value;
        var json = {
            "event": "setSettings",
            "context": uuid,
            "payload": {
                "settingsModel": settingsModel
            }
        };
        websocket.send(JSON.stringify(json));
    }
};

