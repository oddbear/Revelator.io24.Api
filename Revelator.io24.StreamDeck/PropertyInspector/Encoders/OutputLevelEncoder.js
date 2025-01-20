/// <reference path="../SDPIComponentsTypeDef.js" />

document.addEventListener('DOMContentLoaded', async function () {
    const { streamDeckClient } = SDPIComponents;

    const infoResult = await streamDeckClient.getConnectionInfo();
    const controller = infoResult.actionInfo.payload.controller;
    const isEncoder = controller === "Encoder";
    const isKeypad = controller === "Keypad";

    // Set initial values:
    // A encoder will never be a keypad, therefor we can do some assumptions:
    if (isEncoder) {

        // Encoder is always adjust (and can be hidden):
        select_action.value = "adjust";
    }
    else {
        // SDPI Select Elements:
        const sdpi_select_output = document.querySelector('sdpi-select[setting="output"]');
        const sdpi_select_action = document.querySelector('sdpi-select[setting="action"]');

        // HTML Select Elements:
        const select_output = sdpi_select_output.shadowRoot.querySelector('select');
        const select_action = sdpi_select_action.shadowRoot.querySelector('select');

        // Adding changed events:
        select_output.addEventListener('change', changeEvent);
        select_action.addEventListener('change', changeEvent);

        // Keypad can adjust and set:
        setSdpiVisibility("action", true);

        // Trigger initial change event:
        changeEvent();

        function changeEvent() {
            setSdpiVisibility("volume-set", isVolumeOutput() && isSetAction());
            setSdpiVisibility("volume-adjust", isVolumeOutput() && isAdjustAction());
            setSdpiVisibility("blend-set", isBlendOutput() && isSetAction());
            setSdpiVisibility("blend-adjust", isBlendOutput() && isAdjustAction());
        }

        function isVolumeOutput() {
            return select_output.value === "mainOut"
                || select_output.value === "phones";
        }

        function isBlendOutput() {
            return select_output.value === "blend";
        }

        function isSetAction() {
            return isKeypad && select_action.value === "set";
        }

        function isAdjustAction() {
            return isEncoder || select_action.value === "adjust";
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