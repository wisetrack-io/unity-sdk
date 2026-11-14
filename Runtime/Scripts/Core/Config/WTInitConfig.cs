using System;
using System.Collections.Generic;

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
    public bool AutoTrackScenes = true;

    /// <summary>
    /// Diagnostics for AI coding agents during integration. When true and <see cref="UserEnvironment"/>
    /// is Sandbox, the native Android SDK writes one <c>[WT-AGENT] {json}</c> line per step to Logcat
    /// (tag <c>WiseTrack-Agent</c>), whatever the log level. Secrets, tokens, device ids and parameter
    /// values are never printed. No effect in Production. Remove before release.
    /// </summary>
    public bool AgentDiagnostics = false;

    public Dictionary<string, object> ToMap()
    {
        var map = new Dictionary<string, object>
        {
            { "app_token", AppToken },
            { "client_secret", ClientSecret },
            { "sdk_version", WTResources.SdkVersion },
            { "user_environment", UserEnvironment.ToString().ToLowerInvariant() },
            { "android_store_name", (AndroidStore ?? WTAndroidStore.Other).ToString().ToLowerInvariant() },
            { "ios_store_name", (IOSStore ?? WTIOSStore.Other).ToString().ToLowerInvariant() },
            { "tracking_waiting_time", TrackingWaitingTime },
            { "start_tracker_automatically", StartTrackerAutomatically },
            { "custom_device_id", CustomDeviceId },
            { "default_tracker", DefaultTracker },
            { "deeplink_enabled", DeeplinkEnabled },
            { "log_level", (int)LogLevel },
            { "oaid_enabled", OaidEnabled },
            { "agent_diagnostics", AgentDiagnostics },
        };

        return map;
    }
}
