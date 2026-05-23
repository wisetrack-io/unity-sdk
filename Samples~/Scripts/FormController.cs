using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using Firebase.Messaging;

public class FormController : MonoBehaviour, IWiseTrackLogger
{
    [Header("UI Elements")]
    public TMP_InputField appTokenInput;
    public TMP_InputField clientSecretInput;
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
    public Button trackScreenBtn;
    public Button trackScreenNoParamsBtn;
    public Button goToSecondSceneBtn;
    public ScrollRect scrollRect;


    private bool sdkEnabled = true;
    private bool sdkInitialized = false;
    private bool sdkRunning = false;
    private Firebase.FirebaseApp app;
    private bool firebaseInitialized = false;

    void Start()
    {
        appTokenInput.text = "<AppToken>";
        clientSecretInput.text = "<ClientSecret>";
        eventNameInput.text = "testEvent";
        logLevelField.onValueChanged.AddListener(OnLogLevelChanged);
        startSdkBtn.onClick.AddListener(OnStartSdkClicked);
        pushTokenBtn.onClick.AddListener(OnPushTokenClicked);
        getInfoBtn.onClick.AddListener(OnGetInfoClicked);
        clearDataBtn.onClick.AddListener(OnClearDataClicked);
        createEventBtn.onClick.AddListener(OnCreateEventClicked);
        trackScreenBtn?.onClick.AddListener(OnTrackScreenClicked);
        trackScreenNoParamsBtn?.onClick.AddListener(OnTrackScreenNoParamsClicked);
        goToSecondSceneBtn?.onClick.AddListener(OnGoToSecondSceneClicked);

        WTResources.DefaultSdkEnv = WTSDKEnvironment.Debug;
        WiseTrackLogger.AddLogger(this);

        // Subscribe to deep link events
        WiseTrack.OnDeepLinkReceived += OnDeepLinkReceived;

        // Initialize Firebase
        SetupFirebase();
    }

    void OnDeepLinkReceived(DeepLinkData data)
    {
        string linkType = data.IsDeferred ? "Deferred" : "Direct";
        AddInfoLog($"🔗 [{linkType}] Deep Link: {data.Url}");

        // You can also handle navigation here based on the URL
        // Example: ParseDeepLinkAndNavigate(data.Url);
    }

    void SetupFirebase()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp
                app = Firebase.FirebaseApp.DefaultInstance;
                firebaseInitialized = true;

                // Initialize Firebase Messaging
                InitializeFirebaseMessaging();

                AddInfoLog("Firebase initialized successfully!");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                AddErrorLog($"Firebase dependency error: {dependencyStatus}");
            }
        });
    }

    void InitializeFirebaseMessaging()
    {
        if (!firebaseInitialized) return;

        // Subscribe to token refresh events
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;

        // Request permission for notifications (iOS)
        // #if UNITY_IOS
        FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                AddInfoLog("Firebase Messaging permission granted");
            }
            else
            {
                AddErrorLog("Firebase Messaging permission denied");
            }
        });
        // #endif

        // Get the current token
        GetFcmToken();
    }

    void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        AddInfoLog($"FCM Token refreshed: {token.Token}");

        // Update the token in WiseTrack
        WiseTrack.SetFCMToken(token.Token);
    }

    void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        AddInfoLog($"Received FCM message: {e.Message.MessageId}");

        // Log the notification data
        if (e.Message.Data != null)
        {
            foreach (var kvp in e.Message.Data)
            {
                AddInfoLog($"  Data: {kvp.Key} = {kvp.Value}");
            }
        }

        // Check if this is a WiseTrack notification
        if (WiseTrack.IsWiseTrackNotificationPayload(e.Message.Data))
        {
            AddInfoLog("WiseTrack notification payload detected");
            // WiseTrack handle the notification
            return;
        }

        // Log notification body if available
        if (e.Message.Notification != null)
        {
            AddInfoLog($"  Title: {e.Message.Notification.Title}");
            AddInfoLog($"  Body: {e.Message.Notification.Body}");
        }
    }

    void OnStartSdkClicked()
    {
        if (!sdkInitialized)
        {
            string appToken = appTokenInput.text;
            string clientSecret = clientSecretInput.text;
            string defaultTracker = defaultTrackerInput.text;
            string storeName = storeNameInput.text;

            string levelText = logLevelField.options[logLevelField.value].text;
            WTLogLevel selectedLogLevel = (WTLogLevel)System.Enum.Parse(typeof(WTLogLevel), levelText);

            WTInitialConfig config = new WTInitialConfig
            {
                AppToken = appToken,
                ClientSecret = clientSecret,
                UserEnvironment = WTUserEnvironment.Production,
                AndroidStore = string.IsNullOrEmpty(storeName) ? WTAndroidStore.Other : WTAndroidStore.FromString(storeName),
                IOSStore = string.IsNullOrEmpty(storeName) ? WTIOSStore.Other : WTIOSStore.FromString(storeName),
                DefaultTracker = string.IsNullOrEmpty(defaultTracker) ? null : defaultTracker,
                LogLevel = selectedLogLevel,
            };

            WiseTrack.Initialize(config);
            sdkInitialized = true;
            sdkRunning = true;
            ChangeMainButton("Stop SDK", new Color32(243, 23, 23, 255));
        }
        else
        {
            if (sdkRunning)
            {
                WiseTrack.StopTracking();
                sdkRunning = false;
                ChangeMainButton("Start SDK", new Color32(0, 176, 82, 255));
            }
            else
            {
                sdkRunning = true;
                ChangeMainButton("Stop SDK", new Color32(243, 23, 23, 255));
                WiseTrack.StartTracking();
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
        WiseTrack.SetEnabled(sdkEnabled);
    }

    void OnLogLevelChanged(int value)
    {
        string levelText = logLevelField.options[logLevelField.value].text;
        WTLogLevel selectedLogLevel = (WTLogLevel)System.Enum.Parse(typeof(WTLogLevel), levelText);

        WiseTrack.SetLogLevel(selectedLogLevel);
    }

    void OnClearDataClicked()
    {
        sdkInitialized = false;
        sdkRunning = false;
        WiseTrack.ClearDataAndStop();

        ChangeMainButton("Initial SDK", new Color32(51, 51, 255, 255));
    }

    void OnGetInfoClicked()
    {
        // SetEnabled(bool)
        var referrer = WiseTrack.GetReferrer();
        var adId = WiseTrack.GetAdId();
        var isEnabled = WiseTrack.IsEnabled();
        Debug.Log("SDK.Enabled= " + isEnabled.ToString());
        Debug.Log("SDK.AdID= " + adId);
        Debug.Log("SDK.Referrer= " + referrer);
    }

    void OnPushTokenClicked()
    {
        if (firebaseInitialized)
        {
            // Get the current FCM token
            GetFcmToken();
        }
        else
        {
            AddErrorLog("Firebase not initialized yet");
        }
    }

    void GetFcmToken()
    {
        FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                string token = task.Result;
                AddInfoLog($"Current FCM Token: {token}");
                WiseTrack.SetFCMToken(token);
            }
            else
            {
                AddErrorLog("Failed to get FCM token");
            }
        });
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
        WiseTrack.TrackEvent(Event);
    }

    void OnTrackScreenClicked()
    {
        string screenName = "TestScreen";
        var paramsDict = new Dictionary<string, WTParamValue>
        {
            { "key-1", WTParamValue.FromDynamic(1.1) },
            { "key-2", WTParamValue.FromDynamic(true) },
            { "key-3", WTParamValue.FromDynamic("value-1") }
        };
        var screen = new WTScreen(
            name: screenName,
            type: WTScreenType.Page,
            parameters: paramsDict
        );
        WiseTrack.TrackScreen(screen);
        AddInfoLog($"TrackScreen called: {screenName} (params=1)");
    }

    void OnTrackScreenNoParamsClicked()
    {
        string screenName = "TestScreenNoParam";
        var screen = new WTScreen(
            name: screenName,
            type: WTScreenType.Page
        );
        WiseTrack.TrackScreen(screen);
        AddInfoLog($"TrackScreen called: {screenName} (no params)");
    }

    void OnGoToSecondSceneClicked()
    {
        AddInfoLog("Loading SecondScene...");
        SceneManager.LoadScene("SecondScene");
    }

    void OnDestroy()
    {
        // Unsubscribe from deep link events
        WiseTrack.OnDeepLinkReceived -= OnDeepLinkReceived;

        // Unsubscribe from Firebase events
        if (firebaseInitialized)
        {
            FirebaseMessaging.TokenReceived -= OnTokenReceived;
            FirebaseMessaging.MessageReceived -= OnMessageReceived;
        }
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