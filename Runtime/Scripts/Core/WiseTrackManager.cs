using System;
using System.Threading;
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

/// <summary>
/// Receives callbacks from the native plugin (via <c>UnitySendMessage</c>) on the Unity main thread.
/// The GameObject is created before the first scene loads, so it always exists on the main thread.
/// </summary>
internal class WiseTrackManager : MonoBehaviour
{
    internal const string GameObjectName = "WiseTrackManager";

    private static WiseTrackManager _instance;
    private static int _mainThreadId = -1;

    private static Action<DeepLinkData> _onDeepLink;
    // Deep link received while nobody was subscribed; delivered to the first subscriber.
    private static DeepLinkData _pendingDeepLink;

    // Filled by JsonUtility.
#pragma warning disable 0649
    [Serializable]
    private class NativeLogMessage
    {
        public int level;
        public string tag;
        public string message;
        public string stackTrace;
    }

    [Serializable]
    private class NativeDeepLinkMessage
    {
        public string url;
        public bool isDeferred;
    }
#pragma warning restore 0649

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        EnsureCreated();
    }

    internal static bool IsMainThread
    {
        get { return _mainThreadId == -1 || Thread.CurrentThread.ManagedThreadId == _mainThreadId; }
    }

    internal static void EnsureCreated()
    {
        if (_instance != null) return;

        if (!IsMainThread)
        {
            Debug.LogError("WiseTrack: WiseTrackManager must be created on the Unity main thread.");
            return;
        }

        var go = new GameObject(GameObjectName);
        _instance = go.AddComponent<WiseTrackManager>();
        DontDestroyOnLoad(go);
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

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    internal static void AddDeepLinkListener(Action<DeepLinkData> listener)
    {
        if (listener == null) return;
        _onDeepLink += listener;

        if (_pendingDeepLink != null)
        {
            var pending = _pendingDeepLink;
            _pendingDeepLink = null;
            SafeInvoke(listener, pending);
        }
    }

    internal static void RemoveDeepLinkListener(Action<DeepLinkData> listener)
    {
        if (listener == null) return;
        _onDeepLink -= listener;
    }

    internal static void DispatchDeepLink(DeepLinkData data)
    {
        var handlers = _onDeepLink;
        if (handlers == null)
        {
            _pendingDeepLink = data;
            return;
        }

        foreach (Action<DeepLinkData> handler in handlers.GetInvocationList())
        {
            SafeInvoke(handler, data);
        }
    }

    private static void SafeInvoke(Action<DeepLinkData> handler, DeepLinkData data)
    {
        try
        {
            handler(data);
        }
        catch (Exception e)
        {
            Debug.LogError($"WiseTrack: Exception in deep link listener: {e}");
        }
    }

    /// <summary>
    /// Called from native Android plugin when a log is received.
    /// Message format: JSON <c>{"level":3,"tag":"...","message":"...","stackTrace":"..."}</c>
    /// </summary>
    public void OnLogReceived(string json)
    {
        try
        {
            var log = JsonUtility.FromJson<NativeLogMessage>(json);
            if (log == null) return;

            WiseTrackLogger.OnLogReceived(log.level, log.tag, log.message,
                string.IsNullOrEmpty(log.stackTrace) ? null : log.stackTrace);
        }
        catch (Exception e)
        {
            Debug.LogError($"WiseTrack: Failed to parse log data: {e.Message}");
        }
    }

    /// <summary>
    /// Called from native Android plugin when a deep link is received.
    /// Message format: JSON <c>{"url":"...","isDeferred":false}</c>
    /// </summary>
    public void OnDeepLinkReceived(string json)
    {
        try
        {
            var message = string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<NativeDeepLinkMessage>(json);
            if (message == null || string.IsNullOrEmpty(message.url))
            {
                Debug.LogWarning($"WiseTrack: Invalid deep link message: {json}");
                return;
            }

            DispatchDeepLink(new DeepLinkData(message.url, message.isDeferred));
        }
        catch (Exception e)
        {
            Debug.LogError($"WiseTrack: Failed to parse deep link message: {e.Message}");
        }
    }
}
