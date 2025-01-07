// Function to handle messages from the plugin
function onSendToPropertyInspector(payload) {
    if (payload.hasOwnProperty('isEncoder')) {
        const isEncoder = payload.isEncoder;

        const keyPadItemChangeTypeElement = document.getElementById('keyPad-item-changeType');
        const keyPadItemValueElement = document.getElementById('keyPad-item-value');

        if (isEncoder) {
            keyPadItemChangeTypeElement.style.display = 'none';
            keyPadItemValueElement.style.display = 'none';
        } else {
            keyPadItemChangeTypeElement.style.display = 'flex';
            keyPadItemValueElement.style.display = 'flex';
        }
    }
}

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
