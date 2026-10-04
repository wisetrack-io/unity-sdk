using System;
using System.Collections.Generic;
using UnityEngine;

public interface IWiseTrackLogger
{
    void OnLog(WTLogLevel level, string tag, string message, string stackTrace = null);
}

public static class WiseTrackLogger
{
    private static readonly List<IWiseTrackLogger> _loggers = new List<IWiseTrackLogger>();
    private static bool _isInitialized = false;

    /// <summary>
    /// Raised when the first logger is registered, so native log forwarding is only enabled when
    /// someone actually listens.
    /// </summary>
    internal static event Action FirstLoggerAdded;

    internal static bool HasLoggers
    {
        get { return _loggers.Count > 0; }
    }

    public static void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
    }

    public static void AddLogger(IWiseTrackLogger logger)
    {
        if (logger == null || _loggers.Contains(logger)) return;

        _loggers.Add(logger);
        if (_loggers.Count == 1 && FirstLoggerAdded != null)
        {
            FirstLoggerAdded();
        }
    }

    public static void RemoveLogger(IWiseTrackLogger logger)
    {
        _loggers.Remove(logger);
    }

    public static void OnLogReceived(int level, string tag, string message, string stackTrace = null)
    {
        var logLevel = (WTLogLevel)level;

        // Iterate over a snapshot: a logger may add or remove loggers from inside OnLog.
        foreach (var logger in _loggers.ToArray())
        {
            try
            {
                logger.OnLog(logLevel, tag, message, stackTrace);
            }
            catch (Exception e)
            {
                Debug.LogError($"Logger error: {e.Message}");
            }
        }
    }
}

public class UnityConsoleLogger : IWiseTrackLogger
{
    public void OnLog(WTLogLevel level, string tag, string message, string stackTrace = null)
    {
        string logMessage = $"[{tag}] {message}";

        switch (level)
        {
            case WTLogLevel.Debug:
                Debug.Log(logMessage);
                break;
            case WTLogLevel.Info:
                Debug.Log(logMessage);
                break;
            case WTLogLevel.Warning:
                Debug.LogWarning(logMessage);
                break;
            case WTLogLevel.Error:
                Debug.LogError(logMessage + (stackTrace != null ? "\n" + stackTrace : ""));
                break;
        }
    }
}
