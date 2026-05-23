using UnityEngine;
using UnityEngine.SceneManagement;

internal class WTSceneTracker : MonoBehaviour
{
    private static WTSceneTracker _instance;
    private string _lastTrackedScene;

    public static WTSceneTracker Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("[WiseTrack] SceneTracker");
                _instance = go.AddComponent<WTSceneTracker>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Track initial scene
        var active = SceneManager.GetActiveScene();
        if (active.isLoaded)
            TrackScene(active.name, "initial_load");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)
            TrackScene(scene.name, "scene_load");
    }

    //  string name,
    // WTScreenType type = WTScreenType.Other,
    // string displayName = null,
    // Dictionary<string, WTParamValue> parameters = null,
    // bool isAuto = false,
    // string trigger = null)

    private void TrackScene(string sceneName, string trigger)
    {
        if (sceneName == _lastTrackedScene) return; // simple dedup
        _lastTrackedScene = sceneName;

        WiseTrack.TrackScreen(new WTScreen(name: sceneName, type: WTScreenType.Page, trigger: trigger, isAuto: true));
    }
}