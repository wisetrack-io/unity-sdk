using System;
using UnityEngine;

/// <summary>
/// Data class for deep link information
/// </summary>
public class DeepLinkData
{
    public string Url { get; private set; }
    public bool IsDeferred { get; private set; }

    public DeepLinkData(string url, bool isDeferred)
    {
        Url = url;
        IsDeferred = isDeferred;
    }
}

public class WiseTrackManager : MonoBehaviour
{
    private static WiseTrackManager _instance;

    /// <summary>
    /// Event fired when a deep link is received from native platform
    /// </summary>
    public static event Action<DeepLinkData> OnDeepLink;

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

    /// <summary>
    /// Called from native Android plugin when a log is received
    /// </summary>
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
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse log data: {e.Message}");
        }
    }

    /// <summary>
    /// Called from native Android plugin when a deep link is received.
    /// Message format: "url|isDeferred"
    /// </summary>
    public void OnDeepLinkReceived(string message)
    {
        try
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning("WiseTrack: Received empty deep link message");
                return;
            }

            var parts = message.Split('|');
            if (parts.Length < 2)
            {
                Debug.LogWarning($"WiseTrack: Invalid deep link message format: {message}");
                return;
            }

            string url = parts[0];
            bool isDeferred = bool.Parse(parts[1].ToLower());

            var deepLinkData = new DeepLinkData(url, isDeferred);

            Debug.Log($"WiseTrack: Deep link received - URL: {url}, IsDeferred: {isDeferred}");

            // Invoke the event for subscribers
            OnDeepLink?.Invoke(deepLinkData);
        }
        catch (Exception e)
        {
            Debug.LogError($"WiseTrack: Exception in deep link listener: {e.Message}");
        }
    }
}
