using System;
using System.Collections.Generic;
using WiseTrack.Core;

namespace WiseTrack.Runtime
{
    [Serializable]
    public class WTInitialConfig
    {
        public string AppToken;
        public WTUserEnvironment UserEnvironment = WTUserEnvironment.Sandbox;
        public WTAndroidStore AndroidStore = WTAndroidStore.Other;
        public WTIOSStore IOSStore = WTIOSStore.Other;
        public int TrackingWaitingTime = 0;
        public bool StartTrackerAutomatically = true;
        public string CustomDeviceId = null;
        public string DefaultTracker = null;
        public string AppSecret = null;
        public string SecretId = null;
        public bool AttributionDeeplink = false;
        public bool EventBuffering = false;
        public WTLogLevel LogLevel = WTResources.DefaultLogLevel;
        public bool OaidEnabled = false;
        public bool ReferrerEnabled = true;

        public Dictionary<string, object> ToMap()
        {
            var map = new Dictionary<string, object>
        {
            { "app_token", AppToken },
            { "sdk_version", WTResources.SdkVersion },
            { "sdk_environment", WTResources.DefaultSdkEnv.ToString().ToLower() },
            { "user_environment", UserEnvironment.ToString().ToLower() },
            { "android_store_name", AndroidStore.ToString().ToLower() },
            { "ios_store_name", IOSStore.ToString().ToLower() },
            { "tracking_waiting_time", TrackingWaitingTime },
            { "start_tracker_automatically", StartTrackerAutomatically },
            { "custom_device_id", CustomDeviceId },
            { "default_tracker", DefaultTracker },
            { "app_secret", AppSecret },
            { "secret_id", SecretId },
            { "attribution_deeplink", AttributionDeeplink },
            { "event_buffering_enabled", EventBuffering },
            { "log_level", (int)LogLevel },
            { "oaid_enabled", OaidEnabled },
            { "referrer_enabled", ReferrerEnabled }
        };

            return map;
        }
    }
}