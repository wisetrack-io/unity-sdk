using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// No-op implementation used in the Editor and on platforms without a native SDK (iOS is not supported yet).
/// Each method logs a single notice the first time it is called, to avoid flooding the console.
/// </summary>
internal class DefaultPlatform : IWiseTrack
{
    private readonly HashSet<string> notified = new HashSet<string>();

    private void Notice(string method, string result = null)
    {
        if (!notified.Add(method)) return;
        Debug.Log($"WiseTrack.{method}() is not supported on this platform (Editor/iOS) - " +
                  (result ?? "call ignored") + ". Further calls will not be logged.");
    }

    public void Initialize(WTInitialConfig wtConfig)
    {
        Notice("Initialize");
    }

    public void EnableNativeLogs()
    {
    }

    public void SetLogLevel(WTLogLevel level)
    {
        Notice("SetLogLevel");
    }

    public void ClearDataAndStop()
    {
        Notice("ClearDataAndStop");
    }

    public void SetEnabled(bool enabled)
    {
        Notice("SetEnabled");
    }

    public bool IsEnabled()
    {
        Notice("IsEnabled", "returning false");
        return false;
    }

    public void StartTracking()
    {
        Notice("StartTracking");
    }

    public void StopTracking()
    {
        Notice("StopTracking");
    }

    public void SetFCMToken(string token)
    {
        Notice("SetFCMToken");
    }

    public void TrackEvent(WTEvent wtEvent)
    {
        Notice("TrackEvent");
    }

    public void TrackScreen(WTScreen wtScreen)
    {
        Notice("TrackScreen");
    }

    public string GetAdId()
    {
        Notice("GetAdId", "returning empty string");
        return "";
    }

    public string GetReferrer()
    {
        Notice("GetReferrer", "returning empty string");
        return "";
    }

    public bool IsWiseTrackNotificationPayload(string payload)
    {
        Notice("IsWiseTrackNotificationPayload", "returning false");
        return false;
    }

    public string GetLastDeeplink()
    {
        Notice("GetLastDeeplink", "returning empty string");
        return "";
    }

    public string GetDeferredDeeplink()
    {
        Notice("GetDeferredDeeplink", "returning empty string");
        return "";
    }
}
