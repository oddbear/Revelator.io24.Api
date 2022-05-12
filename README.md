# PreSonus Revelator io24 API (Unofficial)

This project is to be able to control a PreSonus Revelator io24 interface through an API.
This way we can have **automations** and **plugins** for Stream Deck, Touch Portal, Loupedeck, and more.

This API might also work for other Revelator models (currently only tested with a io44), but limited.

## Progress

- API: Still early, but many features are implemented. Try the Windows App to see what is possible.
- Stream Deck plugin: Fat Channel Toggle, Routing, presets and headphone sources, with status.
- Touch Portal plugin: Volume control, Fat Channel Toggle, Routing, presets and headphone sources, with status.
- Loupedeck plugin: Volume control, Fat Channel Toggle, Routing, presets and headphone sources, with status.

## How to test

- Read the text on this page.
- Go to the [Releases](https://github.com/oddbear/Revelator.io24.Api/releases/tag/v0.0.4-beta) page, or click on `Releases` to the right of this page.
- Also read the text on the releases page.
- Windows App:
  - Download zip file under Assets on the Releases page
  - Unzip
  - Run `Revelator.io24.Wpf.exe`
- Stream Deck plugin:
  - Download `com.oddbear.revelator.io24.streamDeckPlugin` under Assets on the Releases page
  - Install <br /> _(or unzip to plugin directory)_
- Touch Portal plugin:
  - Download `Revelator.io24.TouchPortal.tpp` under Assets on the Releases page
  - Open Touch Portal, click on settings menu and select Import plug-in... <br /> _(or unzip to plugin directory, and restart Touch Portal)_
  - Thrust the plugin, or it wont start
- Loupedeck plugin:
 - Download `Revelator.io24.Loupedeck.lplug4` under Assets on the Releases page
 - Install plugin
 - Activate plugin through the `manage plugins` in the Loupedeck UI

## Warnings

- This API is **unofficial**, at any time, a update to the PreSonus Revelator io24 **can break this API**. 
- This project is in the **early stages**, expect some bugs.
- There will be **breaking changes** between updates.

## About the API

The `Universal Control` and `UC Surface` apps can be used to control some devices from PreSonus. The apps uses a protocol called [UCNET](https://www.presonussoftware.com/en_US/technology) that is not well documented, and easy to use by third parties.

This project aims to document and understand the part of this protocol that is used when comunicating with the io24, and create an API that is more understandable for making plugins, automations and such. There are already similar APIs like this for some other devices, but theese does not seem to work for this particular device.
