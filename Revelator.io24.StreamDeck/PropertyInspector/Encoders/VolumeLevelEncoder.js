// Options:

// Item: Wrapper for layout for one single form element + label etc.
// Control: The actual input element
const warningItem = document.getElementById("warning-item");

// Keypad only:
const actionItem = document.getElementById("action-item");
const volumeItem = document.getElementById("volume-item");

// Both:
const volumeStepItem = document.getElementById("volume-step-item");

const minControl = document.getElementById("min");
const volumeStepControl = document.getElementById("volume-step");
const maxControl = document.getElementById("max");

let isEncoder;

// TODO: Add some Warning if visibility is not set at all (default shown, but hidden on load)
function setVisibility() {
    warningItem.style.display = 'none';

    if (isEncoder) {
        // Set the volume control range:
        minControl.textContent = "-1";

        volumeStepControl.min = 1;
        volumeStepControl.value = payload.volumeStep || 2;
        volumeStepControl.max = 5;

        maxControl.textContent = "+5";
    }
    else {
        // Either volume (absolute) or volume step (relative):
        if (actionItem.value === 'Set') {
            volumeItem.style.display = 'flex';
            volumeStepItem.style.display = 'none';
        }
        else if (actionItem.value === 'Adjust') {
            volumeItem.style.display = 'none';
            volumeStepItem.style.display = 'flex';

            // Set the volume control range:
            minControl.textContent = "-25";

            volumeStepControl.min = -25;
            volumeStepControl.value = payload.volumeStep || 2; // TODO: What's a good number? 3?
            volumeStepControl.max = 25;

            maxControl.textContent = "+25";
        }
        else { // muteControl.isChecked
            volumeItem.style.display = 'none';
            volumeStepItem.style.display = 'none';
        }
    }
}

function onSendToPropertyInspector(payload) {
    if (payload.hasOwnProperty('isEncoder')) {
        // This is only what happens on appear:
        isEncoder = payload.isEncoder;

        const keypadControls = document.querySelectorAll('.keypad');
        const encoderControls = document.querySelectorAll('.encoder');

        // Set all to flex, but have none as default (or you can get flickering on load))
        if (isEncoder) {
            encoderControls.forEach((control) => {
                control.style.display = 'flex';
            });
        } else {
            keypadControls.forEach((control) => {
                control.style.display = 'flex';
            });
        }

        setVisibility();
    }
}

actionItem.addEventListener('change', setVisibility);

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
