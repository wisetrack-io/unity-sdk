# Changelog

## [2.1.0]

### Added

- Added `ClientSecret` field to `WTInitialConfig` for enhanced security and authentication
- Added Deep Linking support with the following features:
  - `DeeplinkEnabled` configuration option to enable/disable deep linking
  - `GetLastDeeplink()` method to retrieve the last received direct deep link
  - `GetDeferredDeeplink()` method to retrieve deferred deep links (for new installs)
  - `SetDeeplinkListener()` method to register a callback for deep link events
  - `OnDeepLinkReceived` event for subscribing to deep link notifications
  - `DeepLinkData` class containing `Url` and `IsDeferred` properties

## [2.0.4]

### Enhanced

- Updated Android Native SDK to `2.2.4`
- Added support for uninstall detection using **Firebase Cloud Messaging (FCM)**.
- Added validation for event parameters to ensure data integrity

## [2.0.3]

### Enhanced

- Updated Android Native SDK to `2.2.2`
- Enhanced event parameter handling and validations
- Updated Android plugin integration

## [2.0.2]

### Chore

- Android SDK updated to `2.1.1`

## [2.0.0]

### Added

- Initial public release of WiseTrack Unity SDK.
- Core tracking functionality.
- Native Android AAR integration.
- Sample scene and usage example.
