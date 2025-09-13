# WiseTrack Unity Package

The **WiseTrack** Unity package provides a solution to accelerate your game or app’s growth, helping you increase users, boost revenue, and reduce costs within the Unity environment.

## Table of Contents

- [Requirements](#requirements)
- [Installation](#installation)
- [Initialization](#initialization)
- [Basic Usage](#basic-usage)
  - [Enabling/Disabling Tracking](#enablingdisabling-tracking)
  - [Starting/Stopping Tracking](#startingstopping-tracking)
  - [Uninstall Detection and Setting Push Notification Tokens](#uninstall-detection-and-setting-push-notification-tokens)
  - [Logging Custom Events](#logging-custom-events)
  - [Setting Log Levels](#setting-log-levels)
  - [Retrieving Advertising IDs](#retrieving-advertising-ids)
- [Advanced Usage](#advanced-usage)
  - [Customizing SDK Behavior](#customizing-sdk-behavior)
- [Example Project](#example-project)
- [Troubleshooting](#troubleshooting)
- [License](#license)

## Requirements

- Unity 2019.4 or later
- Kotlin version 1.9.0 or later
- Android API 21 (Lollipop) or later
- External Dependency Manager for Unity (EDM4U) installed

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

   ### Option 3: Via `.unitypackage` (Manual Import)

   - Go to the [Releases](https://github.com/wisetrack-io/unity-sdk/releases) section of the repository.
   - Download the latest `.unitypackage` file.
   - In Unity, navigate to `Assets > Import Package > Custom Package…`.
   - Select the downloaded `.unitypackage` and click `Import`.
   - Once imported, make sure EDM4U is installed in your project.
   - Trigger dependency resolution via `Assets > External Dependency Manager > Android Resolver > Resolve` if needed.

3. **Configure Android**:
   Ensure your `minSdkVersion` is set to 21 or later

   If your app targets non-Google Play stores (e.g., CafeBazaar, Myket), add these permissions to `Assets/Plugins/Android/AndroidManifest.xml`:

   ```xml
   <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
   <uses-permission android:name="android.permission.READ_PHONE_STATE" />
   ```

   Note: to enable `AndroidManifest.xml` ensure `Custom Main Manifest` is checked in Project Settings.

   ### Feature-Specific Dependencies (Android)

   The WiseTrack Unity Package supports additional Android features that require specific dependencies. Add only the dependencies for the features you need in `Assets/Plugins/Android/mainTemplate.gradle` under the `dependencies` block. If you encounter issues with `appset` or `ads-identifier` dependencies, consider using the [WiseTrackIdentifier Unity Package](https://github.com/wisetrack-io/unity-identifier) as an alternative.

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

4. **Rebuild the Project**:
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

### Starting/Stopping Tracking

Manually control tracking:

```csharp
// Start tracking
WiseTrackBridge.StartTracking();

// Stop tracking
WiseTrackBridge.StopTracking();
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
          WiseTrackBridge.SetFCMToken(token);
      }
    });

    // Or Monitor notification token refresh:
    FirebaseMessaging.TokenReceived += OnTokenReceived;
    void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        WiseTrackBridge.SetFCMToken(token.Token);
    }
```

#### 3. Handle Incoming Notifications

And finally inside your `FirebaseMessaging.MessageReceived` handlers, call the following helper method to check if the message belongs to WiseTrack:

```csharp
    FirebaseMessaging.MessageReceived += OnMessageReceived;
    void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        if (WiseTrackBridge.IsWiseTrackNotificationPayload(e.Message.Data))
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
WiseTrackBridge.LogEvent(Event);

// Log a revenue event
WTEvent Event = WTEvent.RevenueEvent("revenue-event", WTRevenueCurrency.IRR, 120000, Params);
WiseTrackBridge.LogEvent(Event);
```
**Note:** Event parameter keys and values have a maximum limit of 50 characters.

### Setting Log Levels

Control the verbosity of SDK logs:

```csharp
WiseTrackBridge.SetLogLevel(WTLogLevel.Debug); // Options: None, Error, Warning, Info, Debug
```

### Retrieving Advertising IDs

Retrieve the Advertising ID (Ad ID) on Android:

```csharp
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

A Sample project demonstrating the WiseTrack Unity Package integration is available at [Samples~ Folder](https://github.com/wisetrack-io/unity-sdk/tree/main/Samples~).

## Troubleshooting

- **SDK not initializing**: Ensure the `AppToken` is correct and the network is reachable.
- **Logs not appearing**: Set the log level to `WTLogLevel.Debug` and check Console Logs.
- **Ad ID not available**: Ensure Google Play Services is included and the `play-services-ads-identifier` dependency is added (Android).
- **Dependency conflicts**: Use EDM4U to resolve conflicts (`Assets > External Dependency Manager > Android Resolver > Resolve`). If issues persist with `appset` or `ads-identifier`, try the [WiseTrackIdentifier Unity Package](https://github.com/wisetrack-io/unity-identifier).

For further assistance, contact support at [support@wisetrack.io](mailto:support@wisetrack.io).

## License

The WiseTrack Unity Package is licensed under the WiseTrack SDK License Agreement. See the [LICENSE](LICENSE) file for details.
