# WiseTrack Unity Package

The **WiseTrack** Unity package provides a solution to accelerate your game or app’s growth, helping you increase users, boost revenue, and reduce costs within the Unity environment.

## Table of Contents

- [Requirements](#requirements)
- [Installation](#installation)
  - [1. Install External Dependency Manager (EDM4U)](#1-install-external-dependency-manager-edm4u)
  - [2. Add the WiseTrack Package](#2-add-the-wisetrack-package)
  - [3. Configure Android](#3-configure-android)
  - [4. Build](#4-build)
- [Initialization](#initialization)
- [Basic Usage](#basic-usage)
  - [Enabling/Disabling Tracking](#enablingdisabling-tracking)
  - [Starting/Stopping Tracking](#startingstopping-tracking)
  - [Uninstall Detection and Setting Push Notification Tokens](#uninstall-detection-and-setting-push-notification-tokens)
  - [Logging Custom Events](#logging-custom-events)
  - [Screen Tracking](#screen-tracking)
  - [Setting Log Levels](#setting-log-levels)
  - [Receiving SDK Logs](#receiving-sdk-logs)
  - [Retrieving Advertising IDs](#retrieving-advertising-ids)
- [Deep Linking](#deep-linking)
  - [Enable Deep Linking](#enable-deep-linking)
  - [Listening for Deep Links](#listening-for-deep-links)
  - [Retrieving Deep Links](#retrieving-deep-links)
- [Advanced Usage](#advanced-usage)
  - [Customizing SDK Behavior](#customizing-sdk-behavior)
- [Example Project](#example-project)
- [Running the Tests](#running-the-tests)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## Requirements

- Unity 2019.4 or later
- Android API 21 (Lollipop) or later
- Kotlin 1.9.0 or later in the Gradle build (the Unity bridge ships as a Kotlin source file)
- [External Dependency Manager for Unity (EDM4U)](https://github.com/googlesamples/unity-jar-resolver) 1.2.177 or later
- Platforms: **Android**. On iOS and in the Editor every call is a no-op (iOS support is not available yet).

## Installation

### 1. Install External Dependency Manager (EDM4U)

The WiseTrack package uses EDM4U to download the WiseTrack Android SDK (`io.wisetrack.sdk:core`) from Maven Central into your Gradle build. EDM4U is **not** installed automatically, because it is not available in the Unity Registry and many projects already have it (Firebase, ad networks, ...). If it is already in your project, skip this step.

When EDM4U is missing and the build target is Android, WiseTrack logs a warning in the Console once per Editor session.

Pick **one** of the following:

**Option A: OpenUPM (recommended).** Add the scoped registry and the dependency to `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": ["com.google.external-dependency-manager"]
    }
  ],
  "dependencies": {
    "com.google.external-dependency-manager": "1.2.190"
  }
}
```

**Option B: Git URL.** In `Window > Package Manager`, click `+ > Add package from git URL` and enter:

```
https://github.com/googlesamples/unity-jar-resolver.git?path=upm
```

**Option C: `.unitypackage`.** Download `external-dependency-manager-*.unitypackage` from the [EDM4U releases](https://github.com/googlesamples/unity-jar-resolver/releases) and import it via `Assets > Import Package > Custom Package…`.

> Use only one installation method. Having EDM4U both in `Assets/ExternalDependencyManager` and in `Packages/` causes duplicate-class errors.

### 2. Add the WiseTrack Package

**Option 1: Unity Package Manager (Git URL)**

- Open `Window > Package Manager`.
- Click `+ > Add package from git URL` and enter:
  ```
  https://github.com/wisetrack-io/unity-sdk.git
  ```
  To pin a version, append the tag, e.g. `https://github.com/wisetrack-io/unity-sdk.git#v2.3.0`.

**Option 2: `.unitypackage`**

- Download the latest `.unitypackage` from the [Releases](https://github.com/wisetrack-io/unity-sdk/releases) page.
- Import it via `Assets > Import Package > Custom Package…`.

After installing, resolve the Android dependencies: `Assets > External Dependency Manager > Android Resolver > Force Resolve`.

### 3. Configure Android

- Set `Minimum API Level` to 21 or later (`Project Settings > Player > Android > Other Settings`).
- The Android SDK declares the `INTERNET`, `ACCESS_NETWORK_STATE` and `com.google.android.gms.permission.AD_ID` permissions in its own manifest; you do not need to add them.
- If your app targets non-Google Play stores (e.g., CafeBazaar, Myket), add these permissions to `Assets/Plugins/Android/AndroidManifest.xml` (enable `Custom Main Manifest` in `Project Settings > Player > Android > Publishing Settings`):

  ```xml
  <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
  <uses-permission android:name="android.permission.READ_PHONE_STATE" />
  ```

- If your app must not use the advertising ID (for example apps for children), remove the permission in your main manifest:

  ```xml
  <uses-permission android:name="com.google.android.gms.permission.AD_ID" tools:node="remove" />
  ```

#### Feature-Specific Dependencies (Android)

The WiseTrack Android SDK detects the following optional libraries at runtime. Add only the ones for the features you need, in `Assets/Plugins/Android/mainTemplate.gradle` (enable `Custom Main Gradle Template` in `Publishing Settings`) under the `dependencies` block. If you encounter issues with `appset` or `ads-identifier` dependencies, consider using the [WiseTrackIdentifier Unity Package](https://github.com/wisetrack-io/unity-identifier) as an alternative.

- **Google Advertising ID (Ad ID)**: Enables retrieval of the Google Advertising ID via `GetAdId()`.

  ```gradle
  implementation 'com.google.android.gms:play-services-ads-identifier:18.2.0'
  ```

- **AppSet ID**: Provides additional device identification for analytics.

  ```gradle
  implementation 'com.google.android.gms:play-services-appset:16.1.0'
  ```

- **Open Advertising ID (OAID)**: Enables OAID for devices without Google Play Services (e.g., Chinese devices). Also set `OaidEnabled = true` in `WTInitialConfig`.

  ```gradle
  implementation 'io.wisetrack.sdk:oaid:2.0.0' // Replace with the latest version
  ```

- **Huawei Ads Identifier**: Enables Ad ID retrieval on Huawei devices. Add the Huawei repository to `settingsTemplate.gradle` (`dependencyResolutionManagement > repositories`):

  ```gradle
  maven { url 'https://developer.huawei.com/repo/' }
  ```

  ```gradle
  implementation 'com.huawei.hms:ads-identifier:3.4.62.300'
  ```

- **Referrer Tracking**: Enables install referrer tracking for Google Play and CafeBazaar. It is used automatically when the libraries are present.

  ```gradle
  implementation 'io.wisetrack.sdk:referrer:2.0.0' // Replace with the latest version
  implementation 'com.android.installreferrer:installreferrer:2.2' // Google Play referrer
  implementation 'com.github.cafebazaar:referrersdk:1.0.2' // CafeBazaar referrer (needs JitPack)
  ```

  The CafeBazaar referrer is hosted on JitPack; add it to `settingsTemplate.gradle`:

  ```gradle
  maven { url 'https://jitpack.io' }
  ```

- **Firebase Installation ID (FID)**: Enables retrieval of a unique Firebase Installation ID.

  ```gradle
  implementation 'com.google.firebase:firebase-installations:17.2.0'
  ```

  To use Firebase, register your app in the Firebase Console:
  - Add your package name (e.g., `com.example.app`).
  - Download the `google-services.json` file and place it in `Assets/` (the Firebase Unity SDK processes it), or follow the [Firebase Unity setup guide](https://firebase.google.com/docs/unity/setup).

### 4. Build

Build your project to make sure all dependencies are integrated: `File > Build Settings`, select Android, then `Build` or `Build and Run`.

## Initialization

To start using the WiseTrack Unity Package, initialize it with a configuration object in your game’s startup script.

### Example

```csharp
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        var config = new WTInitialConfig
        {
            AppToken = "your-app-token",
            ClientSecret = "your-client-secret",
            UserEnvironment = WTUserEnvironment.Production, // Use Sandbox for testing
            AndroidStore = WTAndroidStore.PlayStore,
            LogLevel = WTLogLevel.Warning
        };

        WiseTrack.Initialize(config);
    }
}
```

**Notes**:
- Replace `"your-app-token"` and `"your-client-secret"` with the credentials provided by the WiseTrack dashboard. Both are required: if either is missing, `Initialize()` logs an error and the SDK is not started.
- Call the WiseTrack API from the Unity main thread (for example, use `ContinueWithOnMainThread` in Firebase callbacks).

## Basic Usage

Below are common tasks you can perform with the WiseTrack Unity Package.

### Enabling/Disabling Tracking

Enable or disable tracking at runtime:

```csharp
// Enable tracking
WiseTrack.SetEnabled(true);

// Disable tracking
WiseTrack.SetEnabled(false);

// Check if tracking is enabled
bool isTrackingEnabled = WiseTrack.IsEnabled();
Debug.Log($"Tracking enabled: {isTrackingEnabled}");
```

### Starting/Stopping Tracking

Manually control tracking:

```csharp
// Start tracking
WiseTrack.StartTracking();

// Stop tracking
WiseTrack.StopTracking();
```


### Uninstall Detection and Setting Push Notification Tokens

To enable WiseTrack Uninstall Detection feature, you need to configure your project to receive push notifications using **Firebase Cloud Messaging (FCM)**.

#### 1. Configure Firebase Cloud Messaging (FCM)

Follow the official Firebase documentation to set up FCM in your project:
👉 [Firebase Cloud Messaging Setup Guide](https://firebase.google.com/docs/unity/setup)

#### 2. Handle Notification Tokens

Once FCM is configured, you need to get Fcm token and pass them to WiseTrack:

```csharp
    FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(task => {
      if (task.IsCompleted && !task.IsFaulted)
      {
          string token = task.Result;
          WiseTrack.SetFCMToken(token);
      }
    });

    // Or Monitor notification token refresh:
    FirebaseMessaging.TokenReceived += OnTokenReceived;
    void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        WiseTrack.SetFCMToken(token.Token);
    }
```

#### 3. Handle Incoming Notifications

And finally inside your `FirebaseMessaging.MessageReceived` handlers, call the following helper method to check if the message belongs to WiseTrack:

```csharp
    FirebaseMessaging.MessageReceived += OnMessageReceived;
    void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        if (WiseTrack.IsWiseTrackNotificationPayload(e.Message.Data))
        {
            // This notification is handled internally by WiseTrack.
            return;
        }
        // Otherwise, handle your app's custom notifications here.
    }
  
```

### Logging Custom Events

Log custom or revenue events:

```csharp
// Log a default event
var Params = new Dictionary<string, WTParamValue>
{
    { "param-1", WTParamValue.FromDynamic("unity") },
    { "param-2", WTParamValue.FromDynamic(2.3) },
    { "param-3", WTParamValue.FromDynamic(true) },
};
WTEvent Event = WTEvent.DefaultEvent("default-event", Params);
WiseTrack.TrackEvent(Event);

// Log a revenue event
WTEvent Event = WTEvent.RevenueEvent("revenue-event", WTRevenueCurrency.IRR, 120000, Params);
WiseTrack.TrackEvent(Event);
```
**Limits:** event name 1-50 characters, parameter keys 1-50 characters, string parameter values up to 100 characters. Invalid events are rejected by the SDK; a warning is also printed in the Unity Console.

Numbers are always sent with an invariant format (`.` as decimal separator), so revenue amounts are tracked correctly on every device language.

### Screen Tracking

WiseTrack supports both automatic scene tracking and manual screen tracking.

#### Auto Tracking (Scenes)

Enabled by default. WiseTrack automatically tracks every Unity scene load (Single mode only). To disable:

```csharp
var config = new WTInitialConfig
{
    AppToken = "your-app-token",
    ClientSecret = "your-client-secret",
    AutoTrackScenes = false
};
WiseTrack.Initialize(config);
```

#### Manual Tracking

For UI panels, menus, dialogs, bottom sheets and state machine navigation. Available types: `Page`, `Dialog`, `BottomSheet`, `Other` (default).

```csharp
// Simple
WiseTrack.TrackScreen(new WTScreen(name: "shop_menu"));

// With type and parameters
WiseTrack.TrackScreen(new WTScreen(
    name: "product_detail",
    type: WTScreenType.Page,
    parameters: new Dictionary<string, WTParamValue>
    {
        ["product_id"] = WTParamValue.FromDynamic("sword_001")
    }
));
```

**Limits:** screen name 1-150 characters, parameter keys 1-50 characters, string parameter values up to 100 characters.

### Setting Log Levels

Control the verbosity of SDK logs:

```csharp
WiseTrack.SetLogLevel(WTLogLevel.Debug); // Options: Debug, Info, Warning, Error
```

The default level is `Warning`. Use `Debug` only while integrating.

### Receiving SDK Logs

Native SDK logs are forwarded to Unity only when at least one logger is registered:

```csharp
// Print SDK logs in the Unity Console
WiseTrackLogger.AddLogger(new UnityConsoleLogger());

// Or implement IWiseTrackLogger to route them to your own logging system
public class MyLogger : IWiseTrackLogger
{
    public void OnLog(WTLogLevel level, string tag, string message, string stackTrace = null) { /* ... */ }
}
```

### Retrieving Advertising IDs

Retrieve the Advertising ID (Ad ID) on Android:

```csharp
// Get Ad ID (Android)
string adId = WiseTrack.GetAdId();
Debug.Log($"Ad ID: {(string.IsNullOrEmpty(adId) ? "Not available" : adId)}");
```

## Deep Linking

WiseTrack SDK supports both **direct deep linking** (when the app is already installed) and **deferred deep linking** (when the app is installed after clicking a link).

### Enable Deep Linking

To enable deep linking, set `DeeplinkEnabled` to `true` in your configuration:

```csharp
var config = new WTInitialConfig
{
    AppToken = "your-app-token",
    ClientSecret = "your-client-secret",
    UserEnvironment = WTUserEnvironment.Production,
    DeeplinkEnabled = true  // Enable deep linking
};

WiseTrack.Initialize(config);
```

### Listening for Deep Links

You can register a callback to be notified when deep links are received. Callbacks run on the Unity main thread.

- A deep link that arrives before anyone has subscribed (for example while your first scene is still loading) is kept and delivered to the first subscriber.
- `SetDeeplinkListener` keeps a single callback: calling it again replaces the previous one, and `null` removes it. Handlers added with `OnDeepLinkReceived +=` are independent of it.

```csharp
using UnityEngine;

public class DeepLinkHandler : MonoBehaviour
{
    void Start()
    {
        // Option 1: Subscribe to the event
        WiseTrack.OnDeepLinkReceived += OnDeepLink;

        // Option 2: Use SetDeeplinkListener method
        WiseTrack.SetDeeplinkListener(data => {
            Debug.Log($"Deep link received: {data.Url}");
        });
    }

    void OnDeepLink(DeepLinkData data)
    {
        Debug.Log($"URL: {data.Url}");
        Debug.Log($"Is Deferred: {data.IsDeferred}");

        if (data.IsDeferred)
        {
            // This is a deferred deep link (user installed app after clicking link)
            Debug.Log("User came from a deferred deep link");
        }
        else
        {
            // This is a direct deep link (app was already installed)
            Debug.Log("User came from a direct deep link");
        }

        // Handle navigation based on the URL
        HandleDeepLinkNavigation(data.Url);
    }

    void HandleDeepLinkNavigation(string url)
    {
        // Your navigation logic here
        // Example: Parse URL and navigate to specific screen
    }

    void OnDestroy()
    {
        // Always unsubscribe when the object is destroyed
        WiseTrack.OnDeepLinkReceived -= OnDeepLink;
    }
}
```

### Retrieving Deep Links

You can also retrieve deep links programmatically:

```csharp
// Get the last received direct deep link
string lastDeepLink = WiseTrack.GetLastDeeplink();
if (!string.IsNullOrEmpty(lastDeepLink))
{
    Debug.Log($"Last Deep Link: {lastDeepLink}");
}

// Get the deferred deep link (from new install)
string deferredDeepLink = WiseTrack.GetDeferredDeeplink();
if (!string.IsNullOrEmpty(deferredDeepLink))
{
    Debug.Log($"Deferred Deep Link: {deferredDeepLink}");
}
```

## Advanced Usage

### Customizing SDK Behavior

Customize the SDK behavior through the `WTInitialConfig` parameters:

- `AppToken`: Your unique app token (required).
- `ClientSecret`: Your client secret key (required).
- `UserEnvironment`: The environment (`Production`, `Sandbox`).
- `AndroidStore`: The Android app store (e.g., `PlayStore`, `CafeBazaar`, `Myket`, `Other`).
- `TrackingWaitingTime`: Delay before starting tracking (in seconds).
- `StartTrackerAutomatically`: Whether to start tracking automatically.
- `CustomDeviceId`: A custom device identifier.
- `DefaultTracker`: A default tracker for event attribution.
- `DeeplinkEnabled`: Enable or disable deep linking support (default: `true`).
- `LogLevel`: Set the initial log level (default: `Warning`).
- `OaidEnabled`: Indicates whether the Open Advertising ID (OAID) is enabled.
- `AutoTrackScenes`: Automatically track Unity scene loads (default: `true`).

Example with advanced configuration:

```csharp
var config = new WTInitialConfig
{
    AppToken = "your-app-token",
    ClientSecret = "your-client-secret",
    UserEnvironment = WTUserEnvironment.Sandbox,
    AndroidStore = WTAndroidStore.PlayStore,
    TrackingWaitingTime = 3,
    StartTrackerAutomatically = true,
    CustomDeviceId = "custom-device-123",
    DefaultTracker = "default-tracker",
    DeeplinkEnabled = true,
    LogLevel = WTLogLevel.Debug,
    OaidEnabled = false
};

WiseTrack.Initialize(config);
```

## Example Project

The package includes a sample (`Window > Package Manager > WiseTrack Unity SDK > Samples > Basic Sample > Import`) with two scenes that demonstrate initialization, events, screen tracking (manual and automatic scene tracking), deep links and logging. Source: [Samples~](https://github.com/wisetrack-io/unity-sdk/tree/main/Samples~).

- The sample uses TextMesh Pro; import the TMP Essential Resources when Unity asks.
- Add `SampleScene` and `SecondScene` to `File > Build Settings` (in this order).
- Firebase Cloud Messaging code is disabled by default, so the sample compiles without Firebase. To enable it, install the Firebase Messaging SDK and add `WISETRACK_FIREBASE` to `Project Settings > Player > Other Settings > Scripting Define Symbols`.

## Running the Tests

The package contains EditMode tests (JSON serialization, models, validation). To run them in your project, add the package to `testables` in `Packages/manifest.json` and open `Window > General > Test Runner`:

```json
{
  "testables": ["io.wisetrack.sdk.unity"]
}
```

## Troubleshooting

- **SDK not initializing**: Ensure `AppToken` and `ClientSecret` are set and correct and the network is reachable. Check the Unity Console for `WiseTrack:` errors.
- **`External Dependency Manager for Unity (EDM4U) was not found` warning**: Install EDM4U, see [step 1](#1-install-external-dependency-manager-edm4u).
- **Logs not appearing**: Register a logger (`WiseTrackLogger.AddLogger(new UnityConsoleLogger())`) and set the log level to `WTLogLevel.Debug`.
- **Ad ID not available**: Ensure Google Play Services is included and the `play-services-ads-identifier` dependency is added (Android).
- **Dependency conflicts**: Use EDM4U to resolve conflicts (`Assets > External Dependency Manager > Android Resolver > Resolve`). If issues persist with `appset` or `ads-identifier`, try the [WiseTrackIdentifier Unity Package](https://github.com/wisetrack-io/unity-identifier).

For further assistance, contact support at [support@wisetrack.io](mailto:support@wisetrack.io).

## License

The WiseTrack Unity Package is licensed under the WiseTrack SDK License Agreement. See the [LICENSE](LICENSE) file for details.
