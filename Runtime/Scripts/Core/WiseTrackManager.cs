using UnityEngine;
using WiseTrack.Runtime;

namespace WiseTrack.Core
{
    public class WiseTrackManager : MonoBehaviour
    {
        private static WiseTrackManager _instance;

        public static WiseTrackManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("WiseTrackManager");
                    _instance = go.AddComponent<WiseTrackManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void OnLogReceived(string logData)
        {
            try
            {
                var parts = logData.Split('|');
                int level = int.Parse(parts[0]);

                string tag = parts[1];
                string message = parts[2];
                string stackTrace = parts.Length > 3 ? parts[3] : null;

                WiseTrackLogger.OnLogReceived(level, tag, message,
                    string.IsNullOrEmpty(stackTrace) ? null : stackTrace);

            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to parse log data: {e.Message}");
            }
        }
    }
}