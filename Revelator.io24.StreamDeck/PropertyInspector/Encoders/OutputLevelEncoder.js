// Warning Box:
const warningDiv = document.getElementById("warning");

// Action Value:
const actionValueSelect = document.getElementById("actionValue");

// Set:
const rangeSetDiv = document.getElementById("rangeSet");
const rangeSetMinValueSpan = document.getElementById("rangeSetMinValue");
const rangeSetValueInputRange = document.getElementById("rangeSetValue");
const rangeSetMaxValueSpan = document.getElementById("rangeSetMaxValue");

// Adjust:
const rangeAdjustDiv = document.getElementById("rangeAdjust");
const rangeAdjustMinValueSpan = document.getElementById("rangeAdjustMinValue");
const rangeAdjustValueInputRange = document.getElementById("rangeAdjustValue");
const rangeAdjustMaxValueSpan = document.getElementById("rangeAdjustMaxValue");

function formatThenSetSettings() {
    // 

    //setSettings();
}

function formatAction() {
    // Need to know prevous option:
    const outputValue = document.getElementById("outputValue");
    const selectedOption = outputValue.options[selectElement.selectedIndex];
    if (!selectedOption) {
        return;
    }

    const actionValue = document.getElementById("actionValue");
    switch (selectedOption.value) {
        case "Set":
            actionValue.querySelector(`option[value="Set"]`).text = "Set Output Volume (dB)";
            actionValue.querySelector(`option[value="Adjust"]`).text = "Adjust Output Volume (dB)";
            break;
        case "Adjust":
        default:
            actionValue.querySelector(`option[value="Set"]`).text = "Set Value";
            actionValue.querySelector(`option[value="Adjust"]`).text = "Adjust Value";
            break;
    }
}

function formatRange() {
    // 
}

function setEncoderVisibility() {
    //actionItem.style.display = 'none';

    //actionValueSelect.options
    //elem.options.length = 0;

    //actionValueSelect.querySelector(`option[value="Set"]`).text = "Set Output Volume (dB)";
    //actionValueSelect.querySelector(`option[value="Adjust"]`).text = "Adjust Output Volume (dB)";

    //// What about ...
    //for (var idx = 0; idx < items.length; idx++) {
    //    var opt = document.createElement('option');
    //    opt.value = items[idx][valueProperty];
    //    opt.text = items[idx][textProperty];
    //    elem.appendChild(opt);
    //}
    //elem.value = payload[valueField];
}

function setKeypadVisibility() {
    //actionItem.style.display = 'flex';
}

// On change
// - not setSettings, but formatThenSetSettings!
// -
// -
// -
function formatAll() {

}

function formatEncoder() {

}

// 1. WebSocket OnConnect -> Plugin
// Any guarantees that this is the correct order always?
// 2. Plugin -> RefreshSettings
// 3. Plugin -> ConnectionOnOnPropertyInspectorDidAppear -> SendToPropertyInspectorAsync

// EasyPi:
// 1. sendValueToPlugin('propertyInspectorConnected'
// Any guarantees that this is actually the first message?
// 2. didReceiveSettings

// Initial load:
function initialLoad(isEncoder) {

    // Remove warning message:
    warningItem.style.display = 'none';

    // - set isEncoder ?

    // - set range values based on isEncoder

    // TODO: Is it possible to convert a keypad to encoder, ex. by moving an existing one?
    // Will it then keep the settings?

    // TODO: Is blend settings already set here?

    // Set:
    //const rangeSetDiv = document.getElementById("rangeSet");
    const rangeSetMinValueSpan = document.getElementById("rangeSetMinValue");
    //const rangeSetValueInputRange = document.getElementById("rangeSetValue");
    const rangeSetMaxValueSpan = document.getElementById("rangeSetMaxValue");

    // Adjust:
    //const rangeAdjustDiv = document.getElementById("rangeAdjust");
    const rangeAdjustMinValueSpan = document.getElementById("rangeAdjustMinValue");
    //const rangeAdjustValueInputRange = document.getElementById("rangeAdjustValue");
    const rangeAdjustMaxValueSpan = document.getElementById("rangeAdjustMaxValue");
    if (isEncoder) {

        // Can switch between Output and Blend:
        // 1 dB to 25 dB for Output, and -1 to +1 for Blend
        rangeSetMinValueSpan.textContent = "1 dB";
        rangeSetMaxValueSpan.textContent = "25 dB";

        // Can switch between Output and Blend:
        // 1 dB to 5 dB for Output, and 0.1 to 0.2 for Blend
        rangeAdjustMinValueSpan.textContent = "1 dB";
        rangeAdjustMaxValueSpan.textContent = "5 dB";

        // Set all .encoder to display: flex;
        const encoderElements = document.querySelectorAll('.encoder');
        encoderElements.forEach(element => {
            element.style.display = 'flex';
        });
    }
    else {
        rangeSetMinValueSpan.textContent = "-96 dB";
        rangeSetMaxValueSpan.textContent = "0 dB";

        rangeAdjustMinValueSpan.textContent = "-25 dB";
        rangeAdjustMaxValueSpan.textContent = "25 dB";

        // Set all .keypad to display: flex;
        const encoderElements = document.querySelectorAll('.keypad');
        encoderElements.forEach(element => {
            element.style.display = 'flex';
        });
    }
}

// When websocket is created, add listener to it for message events:
document.addEventListener('websocketCreate', function () {
    console.log("Websocket created!");

    // When SendToPropertyInspectorAsync is called FROM the plugin
    websocket.addEventListener('message', function (event) {
        warningDiv.style.display = 'none';

        const json = JSON.parse(event.data);

        if (json.event === 'sendToPropertyInspector') {
            const payload = json.payload;

            if (payload.hasOwnProperty('isEncoder')) {

                initialLoad(payload.isEncoder);
            }
        }
    });
});
