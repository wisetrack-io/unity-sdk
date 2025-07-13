using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

#if UNITY_ANDROID
[InitializeOnLoad]
public class WiseTrackPostProcessor
{
    static WiseTrackPostProcessor()
    {
        EditorApplication.delayCall += () =>
        {
            try
            {
                var resolverType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => {
                        try { return a.GetTypes(); } catch { return new Type[0]; }
                    })
                    .FirstOrDefault(t => t.FullName == "GooglePlayServices.PlayServicesResolver");

                if (resolverType != null)
                {
                    Debug.Log("WiseTrack: Found External Dependency Manager.");

                    var method = resolverType.GetMethod("Resolve", BindingFlags.Public | BindingFlags.Static);
                    if (method != null)
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length == 2)
                        {
                            method.Invoke(null, new object[] { true, true });
                        }
                        else if (parameters.Length == 1)
                        {
                            method.Invoke(null, new object[] { true });
                        }
                        else if (parameters.Length == 0)
                        {
                            method.Invoke(null, null);
                        }
                        else
                        {
                            Debug.LogWarning($"WiseTrack: Unsupported Resolve() signature with {parameters.Length} parameters.");
                        }

                        Debug.Log("WiseTrack: Dependencies resolved successfully!");
                    }
                    else
                    {
                        Debug.LogWarning("WiseTrack: Resolve() method not found in EDM.");
                    }
                }
                else
                {
                    Debug.LogWarning("WiseTrack: External Dependency Manager not found.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"WiseTrack: Error resolving dependencies: {e.Message}");
            }
        };
    }
}
#endif
