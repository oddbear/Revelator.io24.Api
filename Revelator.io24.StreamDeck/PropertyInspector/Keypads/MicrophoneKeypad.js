/// <reference path="../SDPIComponentsTypeDef.js" />

document.addEventListener("DOMContentLoaded", async function () {
    // SDPI Select Elements:
    const sdpi_select_input = document.querySelector("sdpi-select[setting='input']");
    const sdpi_select_action = document.querySelector("sdpi-select[setting='action']");
    const sdpi_select_profile = document.querySelector("sdpi-select[setting='profile']");

    // HTML Select Elements:
    const select_input = sdpi_select_input.shadowRoot.querySelector("select");
    const select_action = sdpi_select_action.shadowRoot.querySelector("select");
    const select_profile = sdpi_select_profile.shadowRoot.querySelector("select");

    // Adding changed events:
    select_input.addEventListener("change", changeEvent);
    select_action.addEventListener("change", changeEvent);
    select_profile.addEventListener("change", changeEvent);

    // Trigger initial change event:
    changeEvent();

    function changeEvent() {
        // Set select visibilty based on input values:
        setSdpiVisibility("action", hasInputValue());
        setSdpiVisibility("profile", isSetProfileAction());

        const isUsbMic = isMicrophone();
        const hasHotKey = isUsbMic || isLeftInput();

        // Set option visibility based on input values:
        setOptionsVisibility(isUsbMic, hasHotKey);
    }

    function isMicrophone() {
        return select_input.value === "microphone";
    }

    function isLeftInput() {
        return select_input.value === "left";
    }

    // Input Select has a value:
    function hasInputValue() {
        return select_input.value !== "";
    }

    function isSetProfileAction() {
        const validActions = ["profileHotKey", "profilePreset1", "profilePreset2", "profilePreset3", "profilePreset4"];
        return validActions.includes(select_action.value);
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

    /**
     * Option visibility by table
     * @param {bool} isUsbMic
     * @param {bool} hasHotKey
     */
    function setOptionsVisibility(isUsbMic, hasHotKey) {

        // TODO: Whould SELECT be better to be under profiles, as an preset action etc.?

        // Hotkey can only be selected if USB Microphone or Left Microphone on Interface:
        setOptionVisibility("action", "hotkey", hasHotKey);
        setOptionVisibility("action", "profileHotKey", hasHotKey);

        // Preset 3 and 4 can only be selected if USB Microphone:
        setOptionVisibility("action", "preset3", isUsbMic);
        setOptionVisibility("action", "preset4", isUsbMic);
        
        // Profile 7 and 8 cannot be updated:
        setOptionVisibility("profile", "custom7", isUsbMic);
        setOptionVisibility("profile", "custom8", isUsbMic);
    }

    /**
     * Option visibility by select and option
     * @param {string} setting
     * @param {string} optionValue
     * @param {bool} visible
     */
    function setOptionVisibility(setting, optionValue, visible) {
        const sdpi_select = document.querySelector(`sdpi-select[setting='${setting}']`);
        if (!sdpi_select) {
            return;
        }

        const optionElement = sdpi_select.shadowRoot.querySelector(`option[value='${optionValue}']`);
        if (!optionElement) {
            return;
        }

        optionElement.style.display = visible ? "" : "none";
    }
});
