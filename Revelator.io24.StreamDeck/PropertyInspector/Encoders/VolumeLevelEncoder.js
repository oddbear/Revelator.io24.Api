/// <reference path="../SDPIComponentsTypeDef.js" />

document.addEventListener('DOMContentLoaded', async function () {
    const { streamDeckClient } = SDPIComponents;

    const infoResult = await streamDeckClient.getConnectionInfo();
    const controller = infoResult.actionInfo.payload.controller;
    const isEncoder = controller === "Encoder";
    const isKeypad = controller === "Keypad";

    // Set initial values:
    // A encoder will never be a keypad, therefor we can do some assumptions:
    if (isKeypad) {
        // SDPI Select Elements:
        const sdpi_select_mixOut = document.querySelector('sdpi-select[setting="mixOut"]');
        const sdpi_select_input = document.querySelector('sdpi-select[setting="input"]');
        const sdpi_select_action = document.querySelector('sdpi-select[setting="action"]');

        // HTML Select Elements:
        const select_mixOut = sdpi_select_mixOut.shadowRoot.querySelector('select');
        const select_input = sdpi_select_input.shadowRoot.querySelector('select');
        const select_action = sdpi_select_action.shadowRoot.querySelector('select');

        // Adding changed events:
        select_mixOut.addEventListener('change', changeEvent);
        select_input.addEventListener('change', changeEvent);
        select_action.addEventListener('change', changeEvent);

        // Keypad can adjust and set:
        setSdpiVisibility("action", true);

        // Trigger initial change event:
        changeEvent();

        function changeEvent() {
            setSdpiVisibility("volume-set", isSetAction());
            setSdpiVisibility("volume-adjust", isAdjustAction());
            setSdpiVisibility("route-value", isMuteAction());
        }

        function isSetAction() {
            return select_action.value === "set";
        }

        function isAdjustAction() {
            return select_action.value === "adjust";
        }

        function isMuteAction() {
            return select_action.value === "mute";
        }

        /**
         * Get all the settings from the Property Inspector, and set the visibility of the settings.
         * @param {string} setting
         * @param {bool} isVisible
         */
        function setSdpiVisibility(setting, isVisible) {
            const parentElement = document.querySelector(`[setting="${setting}"]`)?.parentElement;
            if (!parentElement) {
                return;
            }

            parentElement.style.display = isVisible ? "flex" : "none";
        }
    }
});












//// Warning Box:
//const warningItem = document.getElementById("warning-item");

//// Action:
//const actionItem = document.getElementById("action-item");
//const actionControl = document.getElementById("actionValue");

//// Volume Range:
//const valueRangeItem = document.getElementById("value-range-item");

//const valueMinControl = document.getElementById("minValue");
//const valueRangeControl = document.getElementById("rangeValue");
//const valueMaxControl = document.getElementById("maxValue");

//// Settings from plugin, set on connection:
//let isEncoder;
//let volume;
//let volumeStep;

//function showRangeItem(min, max, value) {
//    valueRangeItem.style.display = 'flex';

//    valueMinControl.textContent = min;
//    valueMaxControl.textContent = max;

//    valueRangeControl.min = min;
//    valueRangeControl.max = max;
//    valueRangeControl.value = value;
//}

//function setVisibility() {
//    if (isEncoder) {
//        actionItem.style.display = 'none';
//        actionControl.value = 'Adjust';

//        showRangeItem(1, 5, volumeStep);
//    } else {
//        actionItem.style.display = 'flex';

//        switch (actionControl.value) {
//            case 'Set':
//                showRangeItem(-96, +10, volume);
//                break;
//            case 'Adjust':
//                showRangeItem(-25, +25, volumeStep);
//                break;
//            case 'Mute':
//            default:
//                valueRangeItem.style.display = 'none';
//        }
//    }
//}

//function onSendToPropertyInspector(payload) {
//    if (payload.hasOwnProperty('isEncoder')) {
//        // Remove warning message:
//        warningItem.style.display = 'none';

//        // This is only what happens on appear:
//        isEncoder = payload.isEncoder;

//        // Loaded from saved settings:
//        if (payload.action === 'Set') {
//            volume = payload.value;
//            volumeStep = 2;
//        }
//        else {
//            volume = 0;
//            volumeStep = payload.value;
//        }

//        setVisibility();
//    }
//}

//// Add event when changing Action betwen Set, Adjust and Mute:
//actionControl.addEventListener('change', setVisibility);

//// When websocket is created, add listener to it for message events:
//document.addEventListener('websocketCreate', function () {
//    console.log("Websocket created!");

//    // When SendToPropertyInspectorAsync is called FROM the plugin
//    websocket.addEventListener('message', function (event) {
//        // We need the event type and payload, beacuse we don't know if it's an encoder or keypad.
//        // Unless we can get the InitialPayload -> AppearancePayload -> e.Event.Payload (from SDEventReceivedEventArgs<WillAppearEvent> e)
//        const json = JSON.parse(event.data);

//        // Requires the csharp code to send the event, Easy Pi only handles 'didReceiveSettings' event.
//        if (json.event === 'sendToPropertyInspector') {
//            onSendToPropertyInspector(json.payload);
//        }
//    });
//});
