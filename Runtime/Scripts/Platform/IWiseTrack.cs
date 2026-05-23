public interface IWiseTrack
{
    void Initialize(WTInitialConfig wtConfig);
    void SetLogLevel(WTLogLevel level);
    void ClearDataAndStop();
    void SetEnabled(bool enabled);
    bool IsEnabled();
    void StartTracking();
    void StopTracking();
    void SetFCMToken(string token);
    void TrackEvent(WTEvent wtEvent);
    void TrackScreen(WTScreen wtScreen);
    string GetAdId();
    string GetReferrer();
    bool IsWiseTrackNotificationPayload(string payload);
    string GetLastDeeplink();
    string GetDeferredDeeplink();
}
