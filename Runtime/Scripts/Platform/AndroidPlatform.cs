#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using UnityEngine;

/// <summary>
/// Forwards calls to the Kotlin bridge (<c>io.wisetrack.sdk.unity.WiseTrackUnityPlugin</c>).
/// Every call is guarded: a failure on the native side is logged and never thrown into game code.
/// </summary>
internal class AndroidPlatform : IWiseTrack
{
    private const string PluginClassName = "io.wisetrack.sdk.unity.WiseTrackUnityPlugin";

    private readonly AndroidJavaClass pluginClass;
    private bool nativeLogsEnabled;

    public AndroidPlatform()
    {
        // Throws when the class is missing (e.g. stripped by R8); the caller falls back to DefaultPlatform.
        pluginClass = new AndroidJavaClass(PluginClassName);
        Call("ensureInitialized");
    }

    public void Initialize(WTInitialConfig wtConfig)
    {
        string json = JsonParser.Serialize(wtConfig.ToMap());
        Call("initialize", json);
    }

    public void EnableNativeLogs()
    {
        if (nativeLogsEnabled) return;
        nativeLogsEnabled = true;
        Call("addLoggerOutput");
    }

    public void SetLogLevel(WTLogLevel level)
    {
        Call("setLogLevel", (int)level);
    }

    public void ClearDataAndStop()
    {
        Call("clearDataAndStop");
    }

    public void SetEnabled(bool enabled)
    {
        Call("setEnabled", enabled);
    }

    public bool IsEnabled()
    {
        return CallReturn("isEnabled", false);
    }

    public void StartTracking()
    {
        Call("startTracking");
    }

    public void StopTracking()
    {
        Call("stopTracking");
    }

    public void SetFCMToken(string token)
    {
        Call("setFCMToken", token);
    }

    public void TrackEvent(WTEvent wtEvent)
    {
        string json = JsonParser.Serialize(wtEvent.ToMap());
        Call("trackEvent", json);
    }

    public void TrackScreen(WTScreen wtScreen)
    {
        string json = JsonParser.Serialize(wtScreen.ToMap());
        Call("trackScreen", json);
    }

    public string GetAdId()
    {
        return CallReturn<string>("getAdId", null);
    }

    public string GetReferrer()
    {
        return CallReturn<string>("getReferrer", null);
    }

    public bool IsWiseTrackNotificationPayload(string payload)
    {
        return CallReturn("isWiseTrackNotificationPayload", false, payload);
    }

    public string GetLastDeeplink()
    {
        return CallReturn<string>("getLastDeeplink", null);
    }

    public string GetDeferredDeeplink()
    {
        return CallReturn<string>("getDeferredDeeplink", null);
    }

    private void Call(string method, params object[] args)
    {
        try
        {
            pluginClass.CallStatic(method, args);
        }
        catch (Exception e)
        {
            Debug.LogError($"WiseTrack: native call '{method}' failed: {e.Message}");
        }
    }

    private T CallReturn<T>(string method, T fallback, params object[] args)
    {
        try
        {
            return pluginClass.CallStatic<T>(method, args);
        }
        catch (Exception e)
        {
            Debug.LogError($"WiseTrack: native call '{method}' failed: {e.Message}");
            return fallback;
        }
    }
}
#endif
