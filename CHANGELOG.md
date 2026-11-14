# Changelog

## [3.0.0]

### Breaking

- Android SDK 3.x (Google Play compliant); builds for other stores need an extra package (see README).
- Revenue currency fix.

### Added

- Diagnostics support for AI-assisted integration.
- Deep link handling improvements.

### Docs

- Documentation updates.

## [2.3.0]

### Enhanced

- Updated Android Native SDK to `2.5.0`
- Unity 2019.4+ compatibility: the C# code no longer uses C# 9 syntax
- Default log level is now `Warning`
- Native logs are forwarded to Unity only when at least one `IWiseTrackLogger` is registered
- Deep link listener is registered before the native SDK initializes, so early deep links are not lost
- A deep link received before anyone subscribed to `OnDeepLinkReceived` is delivered to the first subscriber
- Client-side validation warnings for event/screen names and parameters (same limits as the native SDK)
- EditMode tests for serialization, models and validation
- README: EDM4U installation options, corrected examples and dependency coordinates

### Fixed

- Event, screen and notification payloads are now valid JSON when strings contain quotes, backslashes or control characters
- Numeric values (including revenue amounts) are serialized with the invariant culture; decimals no longer break on devices with Persian, German, Turkish, Russian (etc.) locales
- `WTScreenType.BottomSheet` was tracked as `Other`
- `IsWiseTrackNotificationPayload()` returned `false` for payloads containing quotes
- Integer parameters larger than `int` (`long`) were sent as strings
- Exceptions on the native side no longer propagate into game code; failures are logged instead
- `Initialize()` now validates `AppToken` and `ClientSecret` and logs a clear error instead of failing silently
- The client secret is no longer written to Logcat when initialization fails
- `SetDeeplinkListener()` replaces the previous callback instead of adding another one
- Sample: compiles without Firebase (Firebase code is behind the `WISETRACK_FIREBASE` define), includes `.meta` files and both scenes

### Removed

- `com.google.external-dependency-manager` is no longer a hard dependency in `package.json` (it is not in the Unity Registry and broke installation); install EDM4U as described in the README. A Console warning is shown when it is missing
- Editor `WiseTrackPostProcessor` (forced EDM4U resolution on every script reload)
- Editor `AndroidManifestProcessor` and the package `AndroidManifest.xml`: permissions are already declared by the Android SDK
- `io.sentry.auto-init` manifest entry (it disabled Sentry in host apps)
- Internal types (`JsonParser`, `IWiseTrack`, `AndroidPlatform`, `DefaultPlatform`, `WiseTrackManager`) are no longer public

## [2.2.0]

### Added

- Added `TrackScreen()` method for manual screen tracking (UI panels, menus, dialogs, state machines)
- Added `WTScreen` model with `Name`, `DisplayName`, `Type` (`Page`, `Dialog`, `BottomSheet`, `Other`), and `Params` fields
- Added `WTScreenType` enum for screen presentation style
- Added automatic Unity scene tracking via internal `WTSceneTracker` singleton
  - Listens to `SceneManager.sceneLoaded`, tracks Single-mode loads only
  - Deduplicates consecutive identical scene names
  - Tracks initial scene on `Init()`
- Added `AutoTrackScenes` flag to `WTInitialConfig` (default: `true`) to opt out of automatic scene tracking

## [2.1.1]

### Enhanced

- Updated Android Native SDK to `2.3.1`

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
