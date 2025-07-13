using WiseTrack.Platform;
using WiseTrack.Core;

namespace WiseTrack.Runtime
{
    /// <summary>
    /// WiseTrack SDK main API class
    /// </summary>
    public static class WiseTrackBridge
    {
        private static IWiseTrack platform;

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
    }
}