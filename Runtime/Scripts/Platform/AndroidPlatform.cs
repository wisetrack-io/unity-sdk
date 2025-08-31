using UnityEngine;
using WiseTrack.Runtime;
using WiseTrack.Core;

namespace WiseTrack.Platform
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public class AndroidPlatform : IWiseTrack
    {
        private static AndroidJavaClass pluginClass = new AndroidJavaClass("io.wisetrack.sdk.unity.WiseTrackUnityPlugin");

        public AndroidPlatform(){
            EnsureInitialized();
            AddLoggerOutput();
        }

        public void Initialize(WTInitialConfig wtConfig)
        {
            string json = JsonParser.Serialize(wtConfig.ToMap());
            pluginClass.CallStatic("initialize", json);
        }

        public void EnsureInitialized()
        {
            pluginClass.CallStatic("ensureInitialized");
        }

        public void AddLoggerOutput()
        {
            pluginClass.CallStatic("addLoggerOutput");
        }

        public void SetLogLevel(WTLogLevel level)
        {
            pluginClass.CallStatic("setLogLevel", (int)level);
        }

        public void ClearDataAndStop()
        {
            pluginClass.CallStatic("clearDataAndStop");
        }

        public void SetEnabled(bool enabled)
        {
            pluginClass.CallStatic("setEnabled", enabled);
        }

        public bool IsEnabled()
        {
            return pluginClass.CallStatic<bool>("isEnabled");
        }

        public void StartTracking()
        {
            pluginClass.CallStatic("startTracking");
        }

        public void StopTracking()
        {
            pluginClass.CallStatic("stopTracking");
        }

        public void SetFCMToken(string token)
        {
            pluginClass.CallStatic("setFCMToken", token);
        }

        public void LogEvent(WTEvent wtEvent)
        {
            string json = JsonParser.Serialize(wtEvent.ToMap());
            pluginClass.CallStatic("logEvent", json);
        }

        public string GetAdId()
        {
            return pluginClass.CallStatic<string>("getAdId");
        }

        public string GetReferrer()
        {
            return pluginClass.CallStatic<string>("getReferrer");
        }
    }
#endif
}