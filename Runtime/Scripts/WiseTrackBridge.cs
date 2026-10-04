using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WiseTrack SDK main API class.
/// Call its methods from the Unity main thread.
/// </summary>
public static class WiseTrack
{
    private static readonly object platformLock = new object();
    private static IWiseTrack platform;
    private static Action<DeepLinkData> deeplinkListener;

    /// <summary>
    /// Event fired when a deep link is received.
    /// Subscribe to this event to handle deep links in your application.
    /// A deep link that arrives before anyone subscribes is kept and delivered to the first subscriber.
    /// </summary>
    public static event Action<DeepLinkData> OnDeepLinkReceived
    {
        add { WiseTrackManager.AddDeepLinkListener(value); }
        remove { WiseTrackManager.RemoveDeepLinkListener(value); }
    }

    private static IWiseTrack Platform
    {
        get
        {
            if (platform != null) return platform;

            lock (platformLock)
            {
                if (platform == null)
                {
                    WiseTrackManager.EnsureCreated();
                    platform = CreatePlatform();
                    WiseTrackLogger.FirstLoggerAdded += platform.EnableNativeLogs;
                    if (WiseTrackLogger.HasLoggers) platform.EnableNativeLogs();
                }
            }
            return platform;
        }
    }

    private static IWiseTrack CreatePlatform()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            return new AndroidPlatform();
        }
        catch (Exception e)
        {
            Debug.LogError("WiseTrack: failed to load the Android plugin, the SDK is disabled. " +
                           "Make sure EDM4U resolved the Android dependencies. Error: " + e.Message);
            return new DefaultPlatform();
        }
#else
        return new DefaultPlatform();
#endif
    }

    /// <summary>
    /// Initialize WiseTrack SDK with configuration
    /// </summary>
    /// <param name="wtConfig">WiseTrack configuration object</param>
    public static void Initialize(WTInitialConfig wtConfig)
    {
        var errors = WTValidation.ValidateConfig(wtConfig);
        if (errors.Count > 0)
        {
            Debug.LogError("WiseTrack: Initialize() aborted - " + string.Join("; ", errors.ToArray()));
            return;
        }

        Platform.Initialize(wtConfig);

        if (wtConfig.AutoTrackScenes)
        {
            WTSceneTracker.Instance.Init();
        }
    }

    /// <summary>
    /// Set logging level for WiseTrack SDK
    /// </summary>
    /// <param name="level">Log level</param>
    public static void SetLogLevel(WTLogLevel level)
    {
        Platform.SetLogLevel(level);
    }

    /// <summary>
    /// Check if the notification payload is a WiseTrack notification
    /// </summary>
    /// <param name="data">Message.Data from Firebase notification</param>
    /// <returns>True if it's a WiseTrack notification, false otherwise</returns>
    public static bool IsWiseTrackNotificationPayload(IDictionary<string, string> data)
    {
        if (data == null || data.Count == 0) return false;
        return Platform.IsWiseTrackNotificationPayload(BuildNotificationPayloadJson(data));
    }

    internal static string BuildNotificationPayloadJson(IDictionary<string, string> data)
    {
        var map = new Dictionary<string, object>();
        foreach (var kvp in data)
        {
            // Keep empty values: the payload check on the native side may rely on the key only.
            map[kvp.Key] = kvp.Value ?? "";
        }
        return JsonParser.Serialize(map);
    }

    /// <summary>
    /// Clear all data and stop tracking
    /// </summary>
    public static void ClearDataAndStop()
    {
        Platform.ClearDataAndStop();
    }

    /// <summary>
    /// Enable or disable WiseTrack SDK
    /// </summary>
    /// <param name="enabled">True to enable, false to disable</param>
    public static void SetEnabled(bool enabled)
    {
        Platform.SetEnabled(enabled);
    }

    /// <summary>
    /// Check if WiseTrack SDK is enabled
    /// </summary>
    /// <returns>True if enabled, false otherwise</returns>
    public static bool IsEnabled()
    {
        return Platform.IsEnabled();
    }

    /// <summary>
    /// Start tracking events
    /// </summary>
    public static void StartTracking()
    {
        Platform.StartTracking();
    }

    /// <summary>
    /// Stop tracking events
    /// </summary>
    public static void StopTracking()
    {
        Platform.StopTracking();
    }

    /// <summary>
    /// Set Firebase Cloud Messaging token
    /// </summary>
    /// <param name="token">FCM token</param>
    public static void SetFCMToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogWarning("WiseTrack: SetFCMToken() ignored - token is empty");
            return;
        }
        Platform.SetFCMToken(token);
    }

    /// <summary>
    /// Track an event in WiseTrack
    /// </summary>
    /// <param name="wtEvent">Event object to track</param>
    public static void TrackEvent(WTEvent wtEvent)
    {
        if (wtEvent == null)
        {
            Debug.LogError("WiseTrack: TrackEvent() ignored - event is null");
            return;
        }
        WarnIfInvalid("TrackEvent", WTValidation.ValidateEvent(wtEvent));
        Platform.TrackEvent(wtEvent);
    }

    /// <summary>
    /// Track a screen in WiseTrack
    /// </summary>
    /// <param name="wtScreen">Screen object to track</param>
    public static void TrackScreen(WTScreen wtScreen)
    {
        if (wtScreen == null)
        {
            Debug.LogError("WiseTrack: TrackScreen() ignored - screen is null");
            return;
        }
        WarnIfInvalid("TrackScreen", WTValidation.ValidateScreen(wtScreen));
        Platform.TrackScreen(wtScreen);
    }

    private static void WarnIfInvalid(string method, List<string> errors)
    {
        if (errors.Count == 0) return;
        // The native SDK rejects invalid data; warn here so the problem is visible in the Unity console.
        Debug.LogWarning("WiseTrack: " + method + "() - " + string.Join("; ", errors.ToArray()));
    }

    /// <summary>
    /// Get advertising ID
    /// </summary>
    /// <returns>Advertising ID or empty string if not available</returns>
    public static string GetAdId()
    {
        return Platform.GetAdId();
    }

    /// <summary>
    /// Get referrer information
    /// </summary>
    /// <returns>Referrer string or empty string if not available</returns>
    public static string GetReferrer()
    {
        return Platform.GetReferrer();
    }

    /// <summary>
    /// Get the last received deep link URL.
    /// This can be used to retrieve the most recent direct deep link.
    /// </summary>
    /// <returns>The last deep link URL or empty string if none available</returns>
    public static string GetLastDeeplink()
    {
        return Platform.GetLastDeeplink();
    }

    /// <summary>
    /// Get the deferred deep link URL.
    /// Deferred deep links are received when the app is installed after clicking a link.
    /// </summary>
    /// <returns>The deferred deep link URL or empty string if none available</returns>
    public static string GetDeferredDeeplink()
    {
        return Platform.GetDeferredDeeplink();
    }

    /// <summary>
    /// Set the callback to be notified when deep links are received.
    /// Calling it again replaces the previous callback; pass null to remove it.
    /// Callbacks added with <see cref="OnDeepLinkReceived"/> are not affected.
    /// </summary>
    /// <param name="callback">Callback action that receives DeepLinkData</param>
    public static void SetDeeplinkListener(Action<DeepLinkData> callback)
    {
        if (deeplinkListener != null)
        {
            OnDeepLinkReceived -= deeplinkListener;
        }

        deeplinkListener = callback;

        if (callback != null)
        {
            OnDeepLinkReceived += callback;
        }
    }
}
