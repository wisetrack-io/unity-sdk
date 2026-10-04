using System;
using UnityEditor;
using UnityEngine;

namespace WiseTrackSDK.Editor
{
    /// <summary>
    /// Warns once per Editor session when External Dependency Manager for Unity (EDM4U) is missing.
    /// EDM4U resolves the Android native SDK declared in WiseTrackDependencies.xml; without it the
    /// native SDK is not added to the Gradle build.
    /// </summary>
    [InitializeOnLoad]
    internal static class WiseTrackDependencyCheck
    {
        private const string SessionKey = "WiseTrack.DependencyCheck.Done";
        private const string ReadmeUrl = "https://github.com/wisetrack-io/unity-sdk#installation";

        static WiseTrackDependencyCheck()
        {
            if (SessionState.GetBool(SessionKey, false)) return;
            // Delay until all editor assemblies (including EDM4U's dynamically loaded ones) are loaded.
            EditorApplication.delayCall += Check;
        }

        private static void Check()
        {
            SessionState.SetBool(SessionKey, true);

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android) return;
            if (IsEdmInstalled()) return;

            Debug.LogWarning(
                "WiseTrack: External Dependency Manager for Unity (EDM4U) was not found. " +
                "It is required to add the WiseTrack Android SDK to your build. " +
                "Install it via OpenUPM, the git URL or the .unitypackage as described in " + ReadmeUrl);
        }

        private static bool IsEdmInstalled()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // The dlls are versioned on disk (e.g. Google.JarResolver_v1.2.x.dll) but the assembly names are stable.
                var name = assembly.GetName().Name;
                if (name.StartsWith("Google.JarResolver", StringComparison.Ordinal) ||
                    name.StartsWith("Google.VersionHandler", StringComparison.Ordinal))
                    return true;
            }
            return false;
        }
    }
}
