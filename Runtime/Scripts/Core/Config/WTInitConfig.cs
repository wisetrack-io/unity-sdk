using System;
using System.Collections.Generic;
using WiseTrack.Core;

namespace WiseTrack.Runtime
{
    [Serializable]
    public class WTInitialConfig
    {
        public string AppToken;
        public string ClientSecret;
        public WTUserEnvironment UserEnvironment = WTUserEnvironment.Sandbox;
        public WTAndroidStore AndroidStore = WTAndroidStore.Other;
        public WTIOSStore IOSStore = WTIOSStore.Other;
        public int TrackingWaitingTime = 0;
        public bool StartTrackerAutomatically = true;
        public string CustomDeviceId = null;
        public string DefaultTracker = null;
        public bool DeeplinkEnabled = true;
        public WTLogLevel LogLevel = WTResources.DefaultLogLevel;
        public bool OaidEnabled = false;

        public Dictionary<string, object> ToMap()
        {
            var map = new Dictionary<string, object>
        {
            { "app_token", AppToken },
            { "client_secret", ClientSecret },
            { "sdk_version", WTResources.SdkVersion },
            { "sdk_environment", WTResources.DefaultSdkEnv.ToString().ToLower() },
            { "user_environment", UserEnvironment.ToString().ToLower() },
            { "android_store_name", AndroidStore.ToString().ToLower() },
            { "ios_store_name", IOSStore.ToString().ToLower() },
            { "tracking_waiting_time", TrackingWaitingTime },
            { "start_tracker_automatically", StartTrackerAutomatically },
            { "custom_device_id", CustomDeviceId },
            { "default_tracker", DefaultTracker },
            { "deeplink_enabled", DeeplinkEnabled },
            { "log_level", (int)LogLevel },
            { "oaid_enabled", OaidEnabled },
        };

            return map;
        }
    }
}