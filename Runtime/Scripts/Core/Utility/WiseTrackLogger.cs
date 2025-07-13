// Assets/WiseTrack/Runtime/Scripts/Core/WiseTrackLogger.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WiseTrack.Runtime
{
    public interface IWiseTrackLogger
    {
        void OnLog(WTLogLevel level, string tag, string message, string stackTrace = null);
    }

    public static class WiseTrackLogger
    {
        private static readonly List<IWiseTrackLogger> _loggers = new List<IWiseTrackLogger>();
        private static bool _isInitialized = false;

        public static void Initialize()
        {
            if (_isInitialized) return;

            // // Add default Unity console logger
            // AddLogger(new UnityConsoleLogger());

            // // Initialize platform-specific logger
            // #if UNITY_ANDROID && !UNITY_EDITOR
            // AndroidPlatform.Instance.InitializeLogger();
            // #endif

            _isInitialized = true;
        }

        public static void AddLogger(IWiseTrackLogger logger)
        {
            if (!_loggers.Contains(logger))
            {
                _loggers.Add(logger);
            }
        }

        public static void RemoveLogger(IWiseTrackLogger logger)
        {
            _loggers.Remove(logger);
        }

        public static void OnLogReceived(int level, string tag, string message, string stackTrace = null)
        {
            var logLevel = (WTLogLevel)level;

            foreach (var logger in _loggers)
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
}