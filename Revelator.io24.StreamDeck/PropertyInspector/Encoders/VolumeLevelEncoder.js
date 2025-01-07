// Item: Wrapper for layout for one single form element + label etc.
// Control: The actual input element

// Warning Box:
const warningItem = document.getElementById("warning-item");

// Action:
const actionItem = document.getElementById("action-item");
const actionControl = document.getElementById("actionValue");

// Volume Range:
const valueRangeItem = document.getElementById("value-range-item");

const valueMinControl = document.getElementById("minValue");
const valueRangeControl = document.getElementById("rangeValue");
const valueMaxControl = document.getElementById("maxValue");

// Settings from plugin, set on connection:
let isEncoder;
let volume;
let volumeStep;

function showRangeItem(min, max, value) {
    valueRangeItem.style.display = 'flex';

    valueMinControl.textContent = min;
    valueMaxControl.textContent = max;

    valueRangeControl.min = min;
    valueRangeControl.max = max;
    valueRangeControl.value = value;
}

function setVisibility() {
    if (isEncoder) {
        actionItem.style.display = 'none';
        actionControl.value = 'Adjust';

        showRangeItem(1, 5, volumeStep);
    } else {
        actionItem.style.display = 'flex';

        switch (actionControl.value) {
            case 'Set':
                showRangeItem(-96, +10, volume);
                break;
            case 'Adjust':
                showRangeItem(-25, +25, volumeStep);
                break;
            case 'Mute':
            default:
                valueRangeItem.style.display = 'none';
        }
    }
}

function onSendToPropertyInspector(payload) {
    if (payload.hasOwnProperty('isEncoder')) {
        // Remove warning message:
        warningItem.style.display = 'none';

        // This is only what happens on appear:
        isEncoder = payload.isEncoder;

        // Loaded from saved settings:
        if (payload.action === 'Set') {
            volume = payload.value;
            volumeStep = 2;
        }
        else {
            volume = 0;
            volumeStep = payload.value;
        }

        setVisibility();
    }
}

// Add event when changing Action betwen Set, Adjust and Mute:
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
