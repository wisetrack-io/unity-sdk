using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WiseTrack.Runtime;

public class FormController : MonoBehaviour, IWiseTrackLogger
{
    [Header("UI Elements")]
    public TMP_InputField appTokenInput;
    public TMP_InputField defaultTrackerInput;
    public TMP_InputField storeNameInput;
    public TMP_Dropdown logLevelField;
    public TMP_InputField eventNameInput;
    public TMP_InputField eventParamsInput;
    public TMP_Dropdown eventTypeField;
    public Button createEventBtn;
    public Button pushTokenBtn;
    public Button getInfoBtn;
    public Button clearDataBtn;
    public Button startSdkBtn;
    public ScrollRect scrollRect;


    private bool sdkEnabled = true;
    private bool sdkInitialized = false;
    private bool sdkRunning = false;

    void Start()
    {
        appTokenInput.text = "<AppToken>";
        eventNameInput.text = "testEvent";
        logLevelField.onValueChanged.AddListener(OnLogLevelChanged);
        startSdkBtn.onClick.AddListener(OnStartSdkClicked);
        pushTokenBtn.onClick.AddListener(OnPushTokenClicked);
        getInfoBtn.onClick.AddListener(OnGetInfoClicked);
        clearDataBtn.onClick.AddListener(OnClearDataClicked);
        createEventBtn.onClick.AddListener(OnCreateEventClicked);

        WiseTrackLogger.AddLogger(this);
    }

    void OnStartSdkClicked()
    {
        if (!sdkInitialized)
        {
            string appToken = appTokenInput.text;
            string defaultTracker = defaultTrackerInput.text;
            string storeName = storeNameInput.text;

            string levelText = logLevelField.options[logLevelField.value].text;
            WTLogLevel selectedLogLevel = (WTLogLevel)System.Enum.Parse(typeof(WTLogLevel), levelText);

            WTInitialConfig config = new WTInitialConfig
            {
                AppToken = appToken,
                UserEnvironment = WTUserEnvironment.Production,
                AndroidStore = string.IsNullOrEmpty(storeName) ? WTAndroidStore.Other : WTAndroidStore.FromString(storeName),
                IOSStore = string.IsNullOrEmpty(storeName) ? WTIOSStore.Other : WTIOSStore.FromString(storeName),
                DefaultTracker = string.IsNullOrEmpty(defaultTracker) ? null : defaultTracker,
                LogLevel = selectedLogLevel,
            };

            WiseTrackBridge.Initialize(config);
            sdkInitialized = true;
            sdkRunning = true;
            ChangeMainButton("Stop SDK", new Color32(243, 23, 23, 255));
        }
        else
        {
            if (sdkRunning)
            {
                WiseTrackBridge.StopTracking();
                sdkRunning = false;
                ChangeMainButton("Start SDK", new Color32(0, 176, 82, 255));
            }
            else
            {
                sdkRunning = true;
                ChangeMainButton("Stop SDK", new Color32(243, 23, 23, 255));
                WiseTrackBridge.StartTracking();
            }
        }
    }

    private void ChangeMainButton(string title, Color color)
    {
        Image buttonImage = startSdkBtn.GetComponent<Image>();
        buttonImage.color = color;

        TextMeshProUGUI buttonText = startSdkBtn.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = title;
    }

    void OnToggleEnabledClicked()
    {
        sdkEnabled = !sdkEnabled;
        WiseTrackBridge.SetEnabled(sdkEnabled);
    }

    void OnLogLevelChanged(int value)
    {
        string levelText = logLevelField.options[logLevelField.value].text;
        WTLogLevel selectedLogLevel = (WTLogLevel)System.Enum.Parse(typeof(WTLogLevel), levelText);

        WiseTrackBridge.SetLogLevel(selectedLogLevel);
    }

    void OnClearDataClicked()
    {
        sdkInitialized = false;
        sdkRunning = false;
        WiseTrackBridge.ClearDataAndStop();

        ChangeMainButton("Initial SDK", new Color32(51, 51, 255, 255));
    }

    void OnGetInfoClicked()
    {
        // SetEnabled(bool)
        var referrer = WiseTrackBridge.GetReferrer();
        var adId = WiseTrackBridge.GetAdId();
        var isEnabled = WiseTrackBridge.IsEnabled();
        Debug.Log("SDK.Enabled= " + isEnabled.ToString());
        Debug.Log("SDK.AdID= " + adId);
        Debug.Log("SDK.Referrer= " + referrer);
    }

    void OnPushTokenClicked()
    {
        WiseTrackBridge.SetFCMToken("my-unity-fcm-token");
    }

    void OnCreateEventClicked()
    {
        var eventName = eventNameInput.text.Trim();
        var paramsText = eventParamsInput.text.Trim();

        if (string.IsNullOrEmpty(eventName)) return;

        var paramsDict = new Dictionary<string, WTParamValue>();
        string[] parameters = paramsText.Split(',');
        foreach (var item in parameters)
        {
            if (!item.Contains('=')) continue;
            string[] parts = item.Trim().Split('=');
            string key = parts[0];
            string valueString = parts[1];

            if (int.TryParse(valueString, out int resInt))
            {
                paramsDict.Add(key, WTParamValue.FromDynamic(resInt));
                continue;
            }
            if (decimal.TryParse(valueString, out decimal resDecimal))
            {
                paramsDict.Add(key, WTParamValue.FromDynamic(resDecimal));
                continue;
            }
            if (float.TryParse(valueString, out float resFloat))
            {
                paramsDict.Add(key, WTParamValue.FromDynamic(resFloat));
                continue;
            }
            if (double.TryParse(valueString, out double resDouble))
            {
                paramsDict.Add(key, WTParamValue.FromDynamic(resDouble));
                continue;
            }
            if (bool.TryParse(valueString, out bool resBool))
            {
                paramsDict.Add(key, WTParamValue.FromDynamic(resBool));
                continue;
            }
            paramsDict.Add(key, WTParamValue.FromDynamic(valueString));
        }

        WTEvent Event;
        var eType = eventTypeField.options[eventTypeField.value].text;
        switch (eType)
        {
            case "Default":
                Event = WTEvent.DefaultEvent(eventName, paramsDict);
                break;
            case "Revenue":
                Event = WTEvent.RevenueEvent(eventName, WTRevenueCurrency.IRR, 120000, paramsDict);
                break;
            default:
                return;
        }
        WiseTrackBridge.LogEvent(Event);
    }

    public void AddLog(string logText, Color? textColor = null)
    {
        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect reference is null!");
            return;
        }

        Transform content = scrollRect.content;
        if (content == null)
        {
            Debug.LogError("ScrollView content is null!");
            return;
        }

        SetupScrollViewIfNeeded();
        GameObject logObject = CreateLogObject(content, logText, textColor ?? Color.black);
        StartCoroutine(UpdateLayoutAndScroll());
    }

    private void SetupScrollViewIfNeeded()
    {
        Transform content = scrollRect.content;

        if (content.GetComponent<VerticalLayoutGroup>() != null)
            return;

        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.vertical = true;
        scrollRect.horizontal = false;
        scrollRect.scrollSensitivity = 20f;

        VerticalLayoutGroup layoutGroup = content.gameObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.spacing = 5f;

        ContentSizeFitter contentFitter = content.gameObject.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
    }

    private GameObject CreateLogObject(Transform parent, string text, Color textColor)
    {
        GameObject logObject = new GameObject("Log_" + System.DateTime.Now.Ticks);
        logObject.transform.SetParent(parent);

        RectTransform rectTransform = logObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 1);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(0, 30);
        rectTransform.localScale = Vector3.one;

        Text textComponent = logObject.AddComponent<Text>();
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 14;
        textComponent.color = textColor;
        textComponent.alignment = TextAnchor.MiddleLeft;
        textComponent.text = text;
        textComponent.raycastTarget = false;

        ContentSizeFitter sizeFitter = logObject.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        return logObject;
    }

    private System.Collections.IEnumerator UpdateLayoutAndScroll()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        yield return new WaitForEndOfFrame();
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    public void AddErrorLog(string text) => AddLog(text, new Color32(243, 23, 23, 255));

    public void AddWarningLog(string text) => AddLog(text, new Color32(255, 153, 0, 255));

    public void AddInfoLog(string text) => AddLog(text, new Color32(12, 57, 161, 255));

    public void OnLog(WTLogLevel level, string tag, string message, string stackTrace = null)
    {
        string logEntry = $"{System.DateTime.Now:HH:mm} - {level} | {message}";

        // if (level == WTLogLevel.Error && !string.IsNullOrEmpty(stackTrace))
        // {
        //     logEntry += $"\nStackTrace: {stackTrace}";
        // }

        switch (level)
        {
            case WTLogLevel.Debug:
                AddLog(logEntry);
                break;
            case WTLogLevel.Info:
                AddInfoLog(logEntry);
                break;
            case WTLogLevel.Warning:
                AddWarningLog(logEntry);
                break;
            case WTLogLevel.Error:
                AddErrorLog(logEntry);
                break;
        }
    }
}
