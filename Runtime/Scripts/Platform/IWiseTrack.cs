using System;
using WiseTrack.Runtime;

namespace WiseTrack.Platform
{
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
        void LogEvent(WTEvent wtEvent);
        string GetAdId();
        string GetReferrer();
        bool IsWiseTrackNotificationPayload(string payload);
    }
}