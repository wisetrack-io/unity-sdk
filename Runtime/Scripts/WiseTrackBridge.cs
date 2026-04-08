using System;
using WiseTrack.Platform;
using WiseTrack.Core;
using System.Collections.Generic;

namespace WiseTrack.Runtime
{
    /// <summary>
    /// WiseTrack SDK main API class
    /// </summary>
    public static class WiseTrackBridge
    {
        private static IWiseTrack platform;

        /// <summary>
        /// Event fired when a deep link is received.
        /// Subscribe to this event to handle deep links in your application.
        /// </summary>
        public static event Action<DeepLinkData> OnDeepLinkReceived
        {
            add => WiseTrackManager.OnDeepLink += value;
            remove => WiseTrackManager.OnDeepLink -= value;
        }

        static WiseTrackBridge()
        {
            var manager = WiseTrackManager.Instance;

#if UNITY_ANDROID && !UNITY_EDITOR
            platform = new AndroidPlatform();
#else
            platform = new DefaultPlatform();
#endif
        }

        /// <summary>
        /// Initialize WiseTrack SDK with configuration
        /// </summary>
        /// <param name="wtConfig">WiseTrack configuration object</param>
        public static void Initialize(WTInitialConfig wtConfig)
        {
            platform.Initialize(wtConfig);
        }

        /// <summary>
        /// Set logging level for WiseTrack SDK
        /// </summary>
        /// <param name="level">Log level</param>
        public static void SetLogLevel(WTLogLevel level)
        {
            platform.SetLogLevel(level);
        }

        /// <summary>
        /// Check if the notification payload is a WiseTrack notification
        /// </summary>
        /// <param name="data">Message.Data from Firebase notification</param>
        /// <returns>True if it's a WiseTrack notification, false otherwise</returns>
        public static bool IsWiseTrackNotificationPayload(IDictionary<string, string> data)
        {
            if (data == null || data.Count == 0) return false;
            var jsonParts = new List<string>();
            foreach (var kvp in data)
            {
                jsonParts.Add($"\"{kvp.Key}\":\"{kvp.Value}\"");
            }
            string json = "{" + string.Join(",", jsonParts) + "}";
            return platform.IsWiseTrackNotificationPayload(json);
        }

        /// <summary>
        /// Clear all data and stop tracking
        /// </summary>
        public static void ClearDataAndStop()
        {
            platform.ClearDataAndStop();
        }

        /// <summary>
        /// Enable or disable WiseTrack SDK
        /// </summary>
        /// <param name="enabled">True to enable, false to disable</param>
        public static void SetEnabled(bool enabled)
        {
            platform.SetEnabled(enabled);
        }

        /// <summary>
        /// Check if WiseTrack SDK is enabled
        /// </summary>
        /// <returns>True if enabled, false otherwise</returns>
        public static bool IsEnabled()
        {
            return platform.IsEnabled();
        }

        /// <summary>
        /// Start tracking events
        /// </summary>
        public static void StartTracking()
        {
            platform.StartTracking();
        }

        /// <summary>
        /// Stop tracking events
        /// </summary>
        public static void StopTracking()
        {
            platform.StopTracking();
        }

        /// <summary>
        /// Set Firebase Cloud Messaging token
        /// </summary>
        /// <param name="token">FCM token</param>
        public static void SetFCMToken(string token)
        {
            platform.SetFCMToken(token);
        }

        /// <summary>
        /// Log an event to WiseTrack
        /// </summary>
        /// <param name="wtEvent">Event object to log</param>
        public static void LogEvent(WTEvent wtEvent)
        {
            platform.LogEvent(wtEvent);
        }

        /// <summary>
        /// Get advertising ID
        /// </summary>
        /// <returns>Advertising ID or empty string if not available</returns>
        public static string GetAdId()
        {
            return platform.GetAdId();
        }

        /// <summary>
        /// Get referrer information
        /// </summary>
        /// <returns>Referrer string or empty string if not available</returns>
        public static string GetReferrer()
        {
            return platform.GetReferrer();
        }

        /// <summary>
        /// Get the last received deep link URL.
        /// This can be used to retrieve the most recent direct deep link.
        /// </summary>
        /// <returns>The last deep link URL or empty string if none available</returns>
        public static string GetLastDeeplink()
        {
            return platform.GetLastDeeplink();
        }

        /// <summary>
        /// Get the deferred deep link URL.
        /// Deferred deep links are received when the app is installed after clicking a link.
        /// </summary>
        /// <returns>The deferred deep link URL or empty string if none available</returns>
        public static string GetDeferredDeeplink()
        {
            return platform.GetDeferredDeeplink();
        }

        /// <summary>
        /// Set a callback to be notified when deep links are received.
        /// This is a convenience method - you can also subscribe to OnDeepLinkReceived event directly.
        /// </summary>
        /// <param name="callback">Callback action that receives DeepLinkData</param>
        public static void SetDeeplinkListener(Action<DeepLinkData> callback)
        {
            if (callback != null)
            {
                OnDeepLinkReceived += callback;
            }
        }
    }
}