# WiseTrack Unity Package

The **WiseTrack** Unity package provides a solution to accelerate your game or app’s growth, helping you increase users, boost revenue, and reduce costs within the Unity environment.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Initialization](#initialization)
- [Basic Usage](#basic-usage)
  - [Enabling/Disabling Tracking](#enablingdisabling-tracking)
  - [Requesting App Tracking Transparency (ATT) Permission (iOS)](#requesting-app-tracking-transparency-att-permission-ios)
  - [Starting/Stopping Tracking](#startingstopping-tracking)
  - [Setting Push Notification Tokens](#setting-push-notification-tokens)
  - [Logging Custom Events](#logging-custom-events)
  - [Setting Log Levels](#setting-log-levels)
  - [Retrieving Advertising IDs](#retrieving-advertising-ids)
- [Advanced Usage](#advanced-usage)
  - [Customizing SDK Behavior](#customizing-sdk-behavior)
- [Example Project](#example-project)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## Features

- Cross-platform tracking for Android (iOS support coming soon)
- Support for custom and revenue event logging
- Push notification token management (FCM for Android)
- Configurable logging levels
- Advertising ID retrieval (Ad ID for Android, OAID for non-Google Play devices)
- Referrer tracking for attribution

## Requirements

- Unity 2019.4 or later
- Android API 21 (Lollipop) or later
- External Dependency Manager for Unity (EDM4U) installed
- Android Gradle Plugin >= 7.1.0 for full compatibility with Java 17
- iOS 11.0 or later (support coming soon, not yet implemented)

## Installation

To integrate the WiseTrack Unity Package into your Unity project, follow these steps:

1. **Install External Dependency Manager for Unity (EDM4U)**:
   The WiseTrack Unity Package requires EDM4U to manage Android and iOS dependencies. Follow the installation instructions provided in the [EDM4U GitHub repository](https://github.com/googlesamples/unity-jar-resolver).

2. **Add the WiseTrack Package**:
   You can add the package via Unity Package Manager (UPM) or directly from a Git URL.

   ### Option 1: Via Unity Package Manager (UPM)

   - Open Unity and navigate to `Window > Package Manager`.
   - Click the `+` button in the top-left corner and select `Add package from git URL`.
   - Enter the following URL (replace with the actual GitHub URL for your package):
     ```
     https://github.com/wisetrack-io/unity-sdk.git
     ```
   - Click `Add`. Unity will download and install the package.
   - If prompted, resolve dependencies using EDM4U by navigating to `Assets > External Dependency Manager > Android Resolver > Force Resolve`.

   ### Option 2: Via GitHub (Manual Import)

   - Clone or download the WiseTrack Unity package repository from [GitHub](https://github.com/wisetrack-io/unity-sdk).
   - Extract the contents to your project’s `Assets/Plugins/WiseTrack` folder.
   - Open Unity, and EDM4U will automatically detect and resolve dependencies. If not, manually trigger resolution via `Assets > External Dependency Manager > Android Resolver > Resolve`.
   - Ensure all required dependencies are included in your project (see [Feature-Specific Dependencies](#feature-specific-dependencies-android)).

3. **Configure Android**:
   Ensure your `Assets/Plugins/Android/mainTemplate.gradle` has the following settings:

   ```gradle
   android {
       compileSdkVersion 33
       defaultConfig {
           minSdkVersion 21
           targetSdkVersion 33
       }
   }
   ```

   If your app targets non-Google Play stores (e.g., CafeBazaar, Myket), add these permissions to `Assets/Plugins/Android/AndroidManifest.xml`:

   ```xml
   <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
   <uses-permission android:name="android.permission.READ_PHONE_STATE" />
   ```

   ### Feature-Specific Dependencies (Android)

   The WiseTrack Unity Package supports additional Android features that require specific dependencies. Add only the dependencies for the features you need in `Assets/Plugins/Android/mainTemplate.gradle` under the `dependencies` block. If you encounter issues with `appset` or `ads-identifier` dependencies, consider using the [WiseTrackReferrer Unity Package](https://github.com/wisetrack-io/wisetrack-referrer) as an alternative.

   - **Google Advertising ID (Ad ID)**: Enables retrieval of the Google Advertising ID via `GetAdId()`.

     ```gradle
     implementation 'com.google.android.gms:play-services-ads-identifier:18.2.0'
     ```

   - **AppSet ID**: Provides additional device identification for analytics.

     ```gradle
     implementation 'com.google.android.gms:play-services-appset:16.1.0'
     ```

   - **Open Advertising ID (OAID)**: Enables OAID for devices without Google Play Services (e.g., Chinese devices) via `WTInitialConfig` with `oaidEnabled: true`.

     ```gradle
     implementation 'io.wisetrack:sdk:oaid:2.0.0' // Replace with the latest version
     ```

   - **Huawei Ads Identifier**: Enables Ad ID retrieval on Huawei devices.

     ```gradle
     repositories {
         maven { url 'https://developer.huawei.com/repo/' }
     }
     dependencies {
         implementation 'com.huawei.hms:ads-identifier:3.4.62.300'
     }
     ```

   - **Referrer Tracking**: Enables referrer tracking for Google Play and CafeBazaar via `WTInitialConfig` with `referrerEnabled: true`.

     ```gradle
     implementation 'io.wisetrack:sdk:referrer:2.0.0' // Replace with the latest version
     implementation 'com.android.installreferrer:installreferrer:2.2' // Google Play referrer
     implementation 'com.github.cafebazaar:referrersdk:1.0.2' // CafeBazaar referrer
     ```

   - **Firebase Installation ID (FID)**: Enables retrieval of a unique Firebase Installation ID.
     ```gradle
     implementation 'com.google.firebase:firebase-installations:17.2.0'
     ```
     To use Firebase, register your app in the Firebase Console:
     - Add your package name (e.g., `com.example.app`).
     - Download the `google-services.json` file and place it in `Assets/Plugins/Android/`.
     - Update `Assets/Plugins/Android/mainTemplate.gradle`:
       ```gradle
       buildscript {
           dependencies {
               classpath 'com.google.gms:google-services:4.4.1' // Or latest version
           }
       }
       ```
     - Apply the Google Services plugin:
       ```gradle
       apply plugin: 'com.google.gms.google-services'
       ```

4. **Configure iOS**:
   iOS support is not yet implemented but is coming soon. When available, you will need to add the following to `Assets/Plugins/iOS/Info.plist` for App Tracking Transparency (ATT):

   ```xml
   <key>NSUserTrackingUsageDescription</key>
   <string>We use this data to provide a better user experience and personalized ads.</string>
   ```

5. **Rebuild the Project**:
   Build your project to ensure all dependencies are correctly integrated:
   - In Unity, go to `File > Build Settings` and select your target platform.
   - Click `Build` or `Build and Run`.

## Initialization

To start using the WiseTrack Unity Package, initialize it with a configuration object in your game’s startup script.

### Example

```csharp
using UnityEngine;
using WiseTrack.Runtime;

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        var config = new WTInitialConfig
        {
            AppToken = "your-app-token",
            UserEnvironment = WTUserEnvironment.Production, // Use Sandbox for testing
            AndroidStore = WTAndroidStore.GooglePlay,
            iOSStore = WTIOSStore.AppStore,
            LogLevel = WTLogLevel.Warning
        };

        WiseTrackBridge.Initialize(config);
    }
}
```

**Note**: Replace `"your-app-token"` with the token provided by the WiseTrack dashboard.

## Basic Usage

Below are common tasks you can perform with the WiseTrack Unity Package.

### Enabling/Disabling Tracking

Enable or disable tracking at runtime:

```csharp
// Enable tracking
WiseTrackBridge.SetEnabled(true);

// Disable tracking
WiseTrackBridge.SetEnabled(false);

// Check if tracking is enabled
bool isTrackingEnabled = WiseTrackBridge.IsEnabled();
Debug.Log($"Tracking enabled: {isTrackingEnabled}");
```

### Requesting App Tracking Transparency (ATT) Permission (iOS)

For iOS 14+, request user permission for tracking (not yet implemented):

```csharp
// iOS support coming soon
bool isAuthorized = WiseTrackBridge.iOSRequestForATT();
Debug.Log($"Tracking Authorized: {isAuthorized}"); // Placeholder, will return false until implemented
```

**Note**: iOS support is not yet available. This method will return `false` and log a warning until iOS implementation is complete.

### Starting/Stopping Tracking

Manually control tracking:

```csharp
// Start tracking
WiseTrackBridge.StartTracking();

// Stop tracking
WiseTrackBridge.StopTracking();
```

### Setting Push Notification Tokens

Set FCM tokens for push notifications (Android only):

```csharp
// Set FCM token (Android)
WiseTrackBridge.SetFCMToken("your-fcm-token");

// Set APNs token (iOS, not yet implemented)
WiseTrackBridge.SetAPNSToken("your-apns-token"); // Placeholder, no effect until iOS support is added
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
WiseTrackBridge.LogEvent(Event);

// Log a revenue event
WTEvent Event = WTEvent.RevenueEvent("revenue-event", WTRevenueCurrency.IRR, 120000, Params);
WiseTrackBridge.LogEvent(Event);
```

### Setting Log Levels

Control the verbosity of SDK logs:

```csharp
WiseTrackBridge.SetLogLevel(WTLogLevel.Debug); // Options: None, Error, Warning, Info, Debug
```

### Retrieving Advertising IDs

Retrieve the Advertising ID (Ad ID) on Android (IDFA for iOS coming soon):

```csharp
// Get IDFA (iOS, not yet implemented)
string idfa = WiseTrackBridge.GetIdfa();
Debug.Log($"IDFA: {(string.IsNullOrEmpty(idfa) ? "Not available" : idfa)}"); // Placeholder, returns null until implemented

// Get Ad ID (Android)
string adId = WiseTrackBridge.GetAdId();
Debug.Log($"Ad ID: {(string.IsNullOrEmpty(adId) ? "Not available" : adId)}");
```

## Advanced Usage

### Customizing SDK Behavior

Customize the SDK behavior through the `WTInitialConfig` parameters:

- `AppToken`: Your unique app token (required).
- `UserEnvironment`: The environment (`Production`, `Sandbox`).
- `AndroidStore`: The Android app store (e.g., `PlayStore`, `CafeBazaar`, `Myket`, `Other`).
- `iOSStore`: The iOS app store (e.g., `AppStore`, `Sibche`, `Sibapp`, `Anardoni`, `Sibirani`, `Sibjo`, `Other`).
- `TrackingWaitingTime`: Delay before starting tracking (in seconds).
- `StartTrackerAutomatically`: Whether to start tracking automatically.
- `CustomDeviceId`: A custom device identifier.
- `DefaultTracker`: A default tracker for event attribution.
- `LogLevel`: Set the initial log level.
- `OaidEnabled`: Indicates whether the Open Advertising ID (OAID) is enabled.
- `ReferrerEnabled`: Indicates whether the Referrer ID is enabled.

Example with advanced configuration:

```csharp
var config = new WTInitialConfig
{
    AppToken = "your-app-token",
    UserEnvironment = WTUserEnvironment.Sandbox,
    AndroidStore = WTAndroidStore.PlayStore,
    iOSStore = WTIOSStore.AppStore,
    TrackingWaitingTime = 3,
    StartTrackerAutomatically = true,
    CustomDeviceId = "custom-device-123",
    DefaultTracker = "default-tracker",
    LogLevel = WTLogLevel.Debug,
    OaidEnabled = false,
    ReferrerEnabled = true
};

WiseTrackBridge.Init(config);
```

## Example Project

An example project demonstrating the WiseTrack Unity Package integration is available at [GitHub Repository URL](https://github.com/wisetrack-io/unity-sdk/tree/main/example). Clone the repository and follow the setup instructions to see the package in action.

## Troubleshooting

- **SDK not initializing**: Ensure the `AppToken` is correct and the network is reachable.
- **Logs not appearing**: Set the log level to `WTLogLevel.Debug` and check Console Logs.
- **Ad ID not available**: Ensure Google Play Services is included and the `play-services-ads-identifier` dependency is added (Android).
- **Dependency conflicts**: Use EDM4U to resolve conflicts (`Assets > External Dependency Manager > Android Resolver > Resolve`). If issues persist with `appset` or `ads-identifier`, try the [WiseTrackReferrer Unity Package](https://github.com/wisetrack-io/wisetrack-referrer).

For further assistance, contact support at [support@wisetrack.io](mailto:support@wisetrack.io).

## License

The WiseTrack Unity Package is licensed under the WiseTrack SDK License Agreement. See the [LICENSE](LICENSE) file for details.
