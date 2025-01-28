// This file is for VisualStudio intellisense only!
// Include with: /// <reference path="SDPIComponentsTypeDef.js" />

/* eslint-disable */

const SDPIComponents = {
    /** @type {StreamDeckClient} */
    streamDeckClient,

    /** @type {Settings} */
    useGlobalSettings,

    /** @type {Settings} */
    useSettings,

    /** @type {Internationalization} */
    i18n
};

// - Visual Studio does not like @extends or using base type instead of {Object}, extends breaks, and base type overrides child.

/**
 * @typedef {Object} StreamDeckClient
 * @property {EventManager<DidReceiveGlobalSettingsEvent>} didReceiveGlobalSettings
 * @property {EventManager<ActionInfoType | DidReceiveSettingsEvent>} didReceiveSettings
 * @property {EventManager<SendToPropertyInspectorEvent>} sendToPropertyInspector
 * @property {StreamDeckClient_Connect_Callback} connect
 * @property {StreamDeckClient_GetConnectionInfot_Callback} getConnectionInfo
 * @property {StreamDeckClient_GetGlobalSettings_Callback} getGlobalSettings
 * @property {StreamDeckClient_SetGlobalSettings_Callback} setGlobalSettings
 * @property {StreamDeckClient_GetSettingss_Callback} getSettings
 * @property {StreamDeckClient_SetSettings_Callback} setSettings
 * @property {StreamDeckClient_Get_Callback} get
 * @property {StreamDeckClient_Send_Callback} send
 */

/**
 * Gets the connection information used to connect to the Stream Deck.
 * @callback StreamDeckClient_GetConnectionInfot_Callback
 * @returns {Promise<ConnectionInfoType>} - The connection information as a promise.
 */

/**
 * Request the global persistent data.
 * @callback StreamDeckClient_GetGlobalSettings_Callback
 * @returns {Promise<any>} - The global settings as a promise.
 */

/**
 * Gets the settings.
 * @callback StreamDeckClient_GetSettingss_Callback
 * @returns {Promise<any>} - The settings as a promise.
 */

/**
 * Connects to the Stream Deck.
 * @callback StreamDeckClient_Connect_Callback
 * @param {string} port - port The port that should be used to create the WebSocket.
 * @param {string} propertyInspectorUUID - propertyInspectorUUID A unique identifier string to register Property Inspector with Stream Deck software.
 * @param {string} registerEvent - registerEvent The event type that should be used to register the plugin once the WebSocket is opened. For Property Inspector this is "registerPropertyInspector".
 * @param {RegistrationInfoType} info - info The application information.
 * @param {ActionInfoType} actionInfo - actionInfo The action information.
 * @returns {Promise<void>}
 */

/**
 * Save data securely and globally for the plugin.
 * {@link https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setglobalsettings}
 * @callback StreamDeckClient_SetGlobalSettings_Callback
 * @param {any} value - value The global settings.
 * @returns {Promise<void>} - The promise of sending the message that will set the global settings.
 */

/**
 * Save data persistently for the action's instance.
 * {@link https://developer.elgato.com/documentation/stream-deck/sdk/events-sent/#setsettings}
 * @callback StreamDeckClient_SetSettings_Callback
 * @param {any} value - value The settings.
 * @returns {Promise<void>} - The promise of sending the message that will set the action settings.
 */

/**
 * Sends the given `send` event along with the `payload` to the Stream Deck, and continually awaits a response message that matches the `receive` event, and the optional `isComplete` delegate.
 * @callback StreamDeckClient_Get_Callback
 * @param {any} send - send The event to send.
 * @param {any} receive - receive The event to receive.
 * @param {?any} isComplete - isComplete The delegate invokes upon receiving a message from the Stream Deck; when `true`, this promise is fulfilled.
 * @param {?any} payload - payload The optional payload.
 * @returns {Promise<AsEvent>} - The first matching event that fulfilled the `receive` and optional `isComplete` delegate.
 */

/**
 * Sends a message to the Stream Deck.
 * @callback StreamDeckClient_Send_Callback
 * @param {string} event - event The event name.
 * @param {?any} payload - payload The optional payload.
 * @returns {Promise<void>}
 */

/**
 * @typedef {Object} AsEvent - TODO: Add something to this?
 */

/**
 * @template T
 * @typedef {Object} EventManager
 * @property {EventManager_Subscribe_Callback<T>} subscribe
 * @property {EventManager_Unsubscribe_Callback} unsubscribe
 * @property {EventManager_Dispatch_Callback} dispatch
 */

/**
 * Subscribes the handler to the event.
 * @template T
 * @callback EventManager_Subscribe_Callback
 * @param {function(T): void} handler - handler The handler to add.
 * @returns {void}
 */

/**
 * Unsubscribes the handler from the event.
 * @template T
 * @callback EventManager_Unsubscribe_Callback
 * @param {function(T): void} handler - handler The handler to remove.
 * @returns {void}
 */

/**
 * Dispatches the event for all handlers.
 * @template T
 * @callback EventManager_Dispatch_Callback
 * @param {T} args - args The arguments to invoke each handler with.
 * @returns {void}
 */

/**
 * @typedef {Object} EventWithPayloadSettings
 * @property {GenericPayload} payload
 */

/**
 * @typedef {Object} GenericPayload
 * @property {any} settings
 */

/**
 * @typedef {Object} DidReceiveGlobalSettingsEvent
 * @property {"didReceiveGlobalSettings"} event
 * @property {GenericPayload} payload
 */

/**
 * @typedef {Object} DidReceiveSettingsEvent
 * @property {"didReceiveSettings"} event
 * @property {DidReceiveSettingsEventPayload} payload
 */

/**
 * @typedef {Object} SendToPropertyInspectorEvent
 * @property {"sendToPropertyInspector"} event
 * @property {GenericPayload} payload
 */

/**
 * @typedef {Object} DidReceiveSettingsEventPayload
 * @property {boolean} isInMultiAction
 * @property {any} settings
 */

/**
 * @typedef {Object} ActionInfoType
 * @property {string} action
 * @property {string} context
 * @property {string} device
 * @property {CoordinatesPayloadType} payload
 */

/**
 * @typedef {Object} ConnectionInfoType
 * @property {ActionInfoType} actionInfo
 * @property {RegistrationInfoType} info
 * @property {string} propertyInspectorUUID
 * @property {string} registerEvent
 */

/**
 * @typedef {Object} CoordinatesPayloadType
 * @property {CoordinatesType} coordinates
 * @property {any} settings
 */

/**
 * @typedef {Object} CoordinatesType
 * @property {number} column
 * @property {number} row
 */

/**
 * @typedef {Object} PluginType
 * @property {string} version
 */

/**
 * @typedef {Object} ColorType
 * @property {string} buttonPressedBackgroundColor
 * @property {string} buttonPressedBorderColor
 * @property {string} buttonPressedTextColor
 * @property {string} disabledColor
 * @property {string} highlightColor
 * @property {string} mouseDownColor
 */

/**
 * @typedef {Object} RegistrationInfoType
 * @property {ApplicationType} application
 * @property {ColorType} colors
 * @property {number} devicePixelRatio
 * @property {DeviceType[]} devices - array of DeviceType
 * @property {PluginType} plugin
 */

/**
 * @typedef {Object} DeviceType
 * @property {string} id
 * @property {string} name
 * @property {SizeType} sizes
 * @property {DeviceTypeEnum} type - enum
 */

/**
 * @typedef {Object} SizeType
 * @property {number} colums
 * @property {number} rows
 */

/**
 * @typedef {"mac" | "windows"} PlatformType
 */

/**
 * @typedef {Object} ApplicationType
 * @property {string} font
 * @property {string} language
 * @property {PlatformType} platform
 * @property {string} platformVersion
 * @property {string} version
 */

/**
 * @readonly
 * @enum {number}
 */
const DeviceTypeEnum = {
    StreamDeck: 0,
    StreamDeckMini: 1,
    StreamDeckXL: 2,
    StreamDeckMobile: 3,
    CorsairGKeys: 4,
    StreamDeckPedal: 5,
    CorsairVoyager: 6,
    StreamDeckPlus: 7,
    SCUFController: 8,
    StreamDeckNeo: 9
};

/**
 * @typedef {Object} Internationalization
 * @property {InternationalizationType_GetMessage_Callback} getMessage
 * @property {InternationalizationType_GetUILanguage_Callback} getUILanguage
 * @property {string} language 
 * @property {string} fallbackLanguage 
 * @property {?LocalesType} locales
 */

/**
 * Gets the localized string for the specified message. If the message is missing, this method returns an empty string. This is comparable to chrome.i18n.getMessage.
 * {@link https://developer.chrome.com/docs/extensions/reference/i18n/#method-getMessage}
 * @callback InternationalizationType_GetMessage_Callback
 * @param {?string} messageName - messageName The name of the message
 * @returns {string} - Message localized for current locale.
 */

/**
 * Gets the browser UI language of the browser, based on the navigator.
 * {@link https://developer.chrome.com/docs/extensions/reference/i18n/#method-getUILanguage}
 * @callback InternationalizationType_GetUILanguage_Callback
 * @returns {string} - The browser UI language code such as en or fr.
 */

/**
 * @typedef {Object.<string, RecordType>} LocalesType
 */

/**
 * @typedef {Object.<string, string>} RecordType
 */

/**
 * @typedef {Object} Settings
 * @property {GenericPayload} payload
 */
