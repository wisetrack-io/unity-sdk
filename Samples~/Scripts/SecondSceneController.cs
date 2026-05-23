using UnityEngine;
using UnityEngine.SceneManagement;

public class SecondSceneController : MonoBehaviour
{
    private GUIStyle _labelStyle;
    private GUIStyle _btnStyle;

    void OnGUI()
    {
        if (_labelStyle == null)
        {
            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 36, alignment = TextAnchor.MiddleCenter };
            _labelStyle.normal.textColor = Color.white;
            _btnStyle = new GUIStyle(GUI.skin.button) { fontSize = 28 };
        }

        float w = Screen.width;
        float h = Screen.height;

        GUI.Label(new Rect(0, h * 0.25f, w, 80), "Second Scene", _labelStyle);
        GUI.Label(new Rect(0, h * 0.25f + 80, w, 60),
            "Auto-tracked by WiseTrack SceneTracker", _labelStyle);

        float bw = 360, bh = 100;
        if (GUI.Button(new Rect((w - bw) / 2, h * 0.6f, bw, bh), "Back to Main", _btnStyle))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
