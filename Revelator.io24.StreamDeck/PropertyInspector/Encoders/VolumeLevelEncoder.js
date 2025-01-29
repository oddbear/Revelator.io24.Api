/// <reference path="../SDPIComponentsTypeDef.js" />

document.addEventListener("DOMContentLoaded", async function () {
    // eslint-disable-next-line no-undef
    const { streamDeckClient } = SDPIComponents;

    const infoResult = await streamDeckClient.getConnectionInfo();
    const controller = infoResult.actionInfo.payload.controller;

    // eslint-disable-next-line no-unused-vars
    const isEncoder = controller === "Encoder";
    const isKeypad = controller === "Keypad";

    // Set initial values:
    // A encoder will never be a keypad, therefor we can do some assumptions:
    if (isKeypad) {
        // SDPI Select Elements:
        const sdpi_select_mixOut = document.querySelector("sdpi-select[setting='mixOut']");
        const sdpi_select_input = document.querySelector("sdpi-select[setting='input']");
        const sdpi_select_action = document.querySelector("sdpi-select[setting='action']");

        // HTML Select Elements:
        const select_mixOut = sdpi_select_mixOut.shadowRoot.querySelector("select");
        const select_input = sdpi_select_input.shadowRoot.querySelector("select");
        const select_action = sdpi_select_action.shadowRoot.querySelector("select");

        // Adding changed events:
        select_mixOut.addEventListener("change", changeEvent);
        select_input.addEventListener("change", changeEvent);
        select_action.addEventListener("change", changeEvent);

        // Keypad can adjust and set:
        setSdpiVisibility("action", true);

        // Trigger initial change event:
        changeEvent();

        function changeEvent() {
            setSdpiVisibility("volume-set", isSetAction());
            setSdpiVisibility("volume-adjust", isAdjustAction());
            setSdpiVisibility("route-value", isMuteAction() || isSoloAction());
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

        function isSoloAction() {
            return select_action.value === "solo";
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
