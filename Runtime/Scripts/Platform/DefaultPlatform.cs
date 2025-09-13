using UnityEngine;
using WiseTrack.Runtime;

namespace WiseTrack.Platform
{
    public class DefaultPlatform : IWiseTrack
    {
        public void Initialize(WTInitialConfig wtConfig)
        {
            Debug.Log("WiseTrack.Initialize() called in Editor/iOS - functionality disabled");
        }

        protected void AddLoggerOutput()
        {
            Debug.Log("WiseTrack.AddLoggerOutput() called in Editor/iOS - functionality disabled");
        }

        public void SetLogLevel(WTLogLevel level)
        {
            Debug.Log($"WiseTrack.SetLogLevel({level}) called in Editor/iOS - functionality disabled");
        }

        public void ClearDataAndStop()
        {
            Debug.Log("WiseTrack.ClearDataAndStop() called in Editor/iOS - functionality disabled");
        }

        public void SetEnabled(bool enabled)
        {
            Debug.Log($"WiseTrack.SetEnabled({enabled}) called in Editor/iOS - functionality disabled");
        }

        public bool IsEnabled()
        {
            Debug.Log("WiseTrack.IsEnabled() called in Editor/iOS - returning false");
            return false;
        }

        public void StartTracking()
        {
            Debug.Log("WiseTrack.StartTracking() called in Editor/iOS - functionality disabled");
        }

        public void StopTracking()
        {
            Debug.Log("WiseTrack.StopTracking() called in Editor/iOS - functionality disabled");
        }

        public void SetFCMToken(string token)
        {
            Debug.Log($"WiseTrack.SetFCMToken({token}) called in Editor/iOS - functionality disabled");
        }

        public void LogEvent(WTEvent wtEvent)
        {
            Debug.Log($"WiseTrack.LogEvent({wtEvent?.Name}) called in Editor/iOS - functionality disabled");
        }

        public string GetAdId()
        {
            Debug.Log("WiseTrack.GetAdId() called in Editor/iOS - returning empty string");
            return "";
        }

        public string GetReferrer()
        {
            Debug.Log("WiseTrack.GetReferrer() called in Editor/iOS - returning empty string");
            return "";
        }

        public bool IsWiseTrackNotificationPayload(string payload)
        {
            Debug.Log($"WiseTrack.IsWiseTrackNotificationPayload({payload}) called in Editor/iOS - returning false");
            return false;
        }
    }
}