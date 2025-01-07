// Warning Box:
const warningItem = document.getElementById("warning-item");

// Output:
const outputControl = document.getElementById("outputValue");

// Action:
const actionItem = document.getElementById("action-item");
const actionControl = document.getElementById("actionValue");
const actionSetControl = document.getElementById("actionOptionSet");
const actionAdjustControl = document.getElementById("actionOptionAdjust");

// Volume Range:
const valueMinControl = document.getElementById("minValue");
const valueRangeControl = document.getElementById("rangeValue");
const valueMaxControl = document.getElementById("maxValue");

// Settings from plugin, set on connection:
let isEncoder;
let valueOutputSet;
let valueOutputAdjust;
let valueBlendSet;
let valueBlendAdjust;

function showOutputRangeItem(min, max, value) {
    actionSetControl.text = "Set Output Volume (dB)";
    actionAdjustControl.text = "Adjust Output Volume (dB)";

    valueMinControl.textContent = min;
    valueMaxControl.textContent = max;

    valueRangeControl.dataset.suffix = ' dB';
    valueRangeControl.step = 1;
    valueRangeControl.min = min;
    valueRangeControl.max = max;
    valueRangeControl.value = value;
}

function showBlendRangeItem(min, max, value) {
    actionSetControl.text = "Set Value";
    actionAdjustControl.text = "Adjust Value";

    valueMinControl.textContent = min;
    valueMaxControl.textContent = max;

    valueRangeControl.dataset.suffix = '';
    valueRangeControl.step = 0.1;
    valueRangeControl.min = min;
    valueRangeControl.max = max;
    valueRangeControl.value = value;
}

function setEncoderVisibility() {
    actionItem.style.display = 'none';
    actionControl.value = 'Adjust';

    // Encoder:
    switch (outputControl.value) {
        case 'MainOut':
        case 'Phones':
            showOutputRangeItem(1, 5, valueOutputAdjust);
            break;
        case 'Blend':
        default:
            showBlendRangeItem(0.1, 0.2, valueBlendAdjust);
            break;
    }
}

function setKeypadVisibility() {
    actionItem.style.display = 'flex';

    // Keypad:
    switch (outputControl.value) {
        case 'MainOut':
        case 'Phones':
            switch (actionControl.value) {
                case 'Set':
                    showOutputRangeItem(-96, 0, valueOutputSet);
                    break;
                case 'Adjust':
                default:
                    showOutputRangeItem(-25, +25, valueOutputAdjust);
                    break;
            }
            break;
        case 'Blend':
        default:
            switch (actionControl.value) {
                case 'Set':
                    showBlendRangeItem(-1, +1, valueBlendSet);
                    break;
                case 'Adjust':
                default:
                    showBlendRangeItem(-0.2, +0.2, valueBlendAdjust);
                    break;
            }
            break;
    }
}

function setVisibility() {
    if (isEncoder) {
        setEncoderVisibility();
    } else {
        setKeypadVisibility();
    }
}

// Function to handle messages from the plugin
function onSendToPropertyInspector(payload) {
    if (payload.hasOwnProperty('isEncoder')) {
        // Remove warning message:
        warningItem.style.display = 'none';

        // This is only what happens on appear:
        isEncoder = payload.isEncoder;

        // Set default values:
        valueOutputSet = -20;
        valueOutputAdjust = isEncoder ? 2 : 0;
        valueBlendSet = 0;
        valueBlendAdjust = isEncoder ? 0.1 : 0;

        if (payload.device === 'Blend') {
            if (payload.action === 'Set') {
                valueBlendSet = payload.value;
            } else {
                valueBlendAdjust = payload.value;
            }
        } else {
            if (payload.action === 'Set') {
                valueOutputSet = payload.value;
            } else {
                valueOutputAdjust = payload.value;
            }
        }

        // Loaded from saved settings:
        value = payload.value;

        setVisibility();
    }
}

outputControl.addEventListener('change', setVisibility);
actionControl.addEventListener('change', setVisibility);

// When websocket is created, add listener to it for message events:
document.addEventListener('websocketCreate', function () {
    console.log("Websocket created!");

    // When SendToPropertyInspectorAsync is called FROM the plugin
    websocket.addEventListener('message', function (event) {
        // We need the event type and payload, beacuse we don't know if it's an encoder or keypad.
        // Unless we can get the InitialPayload -> AppearancePayload -> e.Event.Payload (from SDEventReceivedEventArgs<WillAppearEvent> e)
        const json = JSON.parse(event.data);

        // Requires the csharp code to send the event, Easy Pi only handles 'didReceiveSettings' event.
        if (json.event === 'sendToPropertyInspector') {
            onSendToPropertyInspector(json.payload);
        }
    });
});
