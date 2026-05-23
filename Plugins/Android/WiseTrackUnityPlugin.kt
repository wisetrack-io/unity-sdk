package io.wisetrack.sdk.unity

import android.app.Activity
import android.content.Intent
import android.util.Log
import org.json.JSONObject
import com.unity3d.player.UnityPlayer
import io.wisetrack.sdk.core.WiseTrack
import io.wisetrack.sdk.core.models.WTParam
import io.wisetrack.sdk.core.models.RevenueCurrency
import io.wisetrack.sdk.core.models.WTEvent
import io.wisetrack.sdk.core.models.WTEventType
import io.wisetrack.sdk.core.models.WTStoreName
import io.wisetrack.sdk.core.models.WTUserEnvironment
import io.wisetrack.sdk.core.models.WTInitialConfig
import io.wisetrack.sdk.core.utils.wrapper.ResourceWrapper
import io.wisetrack.sdk.core.models.WTLogLevel
import io.wisetrack.sdk.core.models.WTScreen
import io.wisetrack.sdk.core.models.WTScreenType
import io.wisetrack.sdk.core.resources.WTLoggerOutput
import io.wisetrack.sdk.core.screen.WTScreenAutoTrackingConfig


object WiseTrackUnityPlugin {

    private const val TAG = "WiseTrackUnityPlugin"

    private fun getActivity(): Activity = UnityPlayer.currentActivity

    @JvmStatic
    fun initialize(jsonConfig: String) {
        try {
            val json = JSONObject(jsonConfig)

            val appToken = json.getString("app_token")
            val clientSecret = json.getString("client_secret")
            val sdkVersion = json.getString("sdk_version")
            val sdkEnv = json.getString("sdk_environment")
            val userEnv = json.getString("user_environment")
            val storeName = json.optString("android_store_name", "other")
            val trackingWaitingTime = json.optInt("tracking_waiting_time", 0)
            val startTrackerAutomatically = json.optBoolean("start_tracker_automatically", true)
            val customDeviceId = if (json.has("custom_device_id")) json.getString("custom_device_id") else null
            val defaultTracker = if (json.has("default_tracker")) json.getString("default_tracker") else null
            val deeplinkEnabled = json.optBoolean("deeplink_enabled", true)
            val logLevel = json.optInt("log_level", 3)
            val oaidEnabled = json.optBoolean("oaid_enabled", false)

            val resourceWrapper = ResourceWrapper(getActivity())
            resourceWrapper.setFramework("unity")
            resourceWrapper.setEnvironment(sdkEnv)
            resourceWrapper.setVersion(sdkVersion)

            val config = WTInitialConfig(
                appToken = appToken,
                clientSecret = clientSecret,
                environment = WTUserEnvironment.valueOf(userEnv.uppercase()),
                storeName = WTStoreName.fromString(storeName),
                trackingWaitingTime = trackingWaitingTime,
                startTrackerAutomatically = startTrackerAutomatically,
                customDeviceId = customDeviceId,
                defaultTracker = defaultTracker,
                deeplinkEnabled = deeplinkEnabled,
                logLevel = WTLogLevel.fromPriority(logLevel),
                oaidEnabled = oaidEnabled,
                screenTrackingConfig = WTScreenAutoTrackingConfig(enabled = false),
            )

            WiseTrack.initialize(getActivity(), config)

            WiseTrack.setOnDeeplinkListener { intent, isDeferred ->
                UnityDeepLinkReceiver.onDeeplinkReceived(intent, isDeferred)
            }

        } catch (e: Exception) {
            Log.e("WiseTrackUnityPlugin", "Failed to parse config: $jsonConfig", e)
        }
    }

    @JvmStatic
    fun isWiseTrackNotificationPayload(jsonData: String): Boolean {
        try {
            val jsonObject = JSONObject(jsonData)
            return WiseTrack.isWiseTrackNotificationPayload(jsonObject.toMap())
        } catch (e: Exception) {
            return false
        }
    }

    @JvmStatic
    fun addLoggerOutput() {
        WiseTrack.addLoggerOutput(UnityLoggerOutput)
    }

    @JvmStatic
    fun ensureInitialized() {
        WiseTrack.ensureInitialized(getActivity())
    }

    @JvmStatic
    fun setLogLevel(level: Int) {
        WiseTrack.setLogLevel(WTLogLevel.fromPriority(level))
    }

    @JvmStatic
    fun clearDataAndStop() {
        WiseTrack.clearDataAndStop()
    }

    @JvmStatic
    fun setEnabled(enabled: Boolean) {
        WiseTrack.setEnabled(enabled)
    }

    @JvmStatic
    fun isEnabled(): Boolean {
        return WiseTrack.isEnabled
    }

    @JvmStatic
    fun startTracking() {
        WiseTrack.startTracking()
    }

    @JvmStatic
    fun stopTracking() {
        WiseTrack.stopTracking()
    }

    @JvmStatic
    fun setFCMToken(token: String?) {
        if (!token.isNullOrEmpty()) {
            WiseTrack.setFCMToken(token)
        }
    }

    @JvmStatic
    fun trackEvent(json: String) {
        val jsonObject = JSONObject(json)
        val name = jsonObject.getString("name")
        val type = jsonObject.getString("type")
        val revenue = jsonObject.optDouble("revenue", 0.0)
        val currency = jsonObject.optString("currency", "USD")

        val paramsJson = jsonObject.optJSONObject("params")
        val paramsMap = mutableMapOf<String, WTParam>()

        if (paramsJson != null) {
            for (key in paramsJson.keys()) {
                paramsMap[key] = when (val value = paramsJson.get(key)) {
                    is Int -> WTParam(value.toDouble())
                    is Double -> WTParam(value)
                    is Boolean -> WTParam(value)
                    else -> WTParam(value.toString())
                }
            }
        }

        val event = if (type.uppercase() == WTEventType.REVENUE.name) {
            WTEvent.revenueEvent(
                name,
                amount = revenue,
                currency = RevenueCurrency.valueOf(currency.uppercase()),
                params = paramsMap
            )
        } else {
            WTEvent.defaultEvent(name, params = paramsMap)
        }

        WiseTrack.trackEvent(event)
    }

    @JvmStatic
    fun trackScreen(json: String) {
        val jsonObject = JSONObject(json)
        val name = jsonObject.getString("name")
        val displayName = jsonObject.optString("display_name")
        val trigger = jsonObject.optString("trigger")

        var screenType: WTScreenType
        try {
            screenType = WTScreenType.valueOf(jsonObject.getString("type").uppercase())
        } catch (e: Throwable) {
            screenType = WTScreenType.OTHER
        }

        val paramsJson = jsonObject.optJSONObject("params")
        val paramsMap = mutableMapOf<String, WTParam>()

        if (paramsJson != null) {
            for (key in paramsJson.keys()) {
                paramsMap[key] = when (val value = paramsJson.get(key)) {
                    is Int -> WTParam(value.toDouble())
                    is Double -> WTParam(value)
                    is Boolean -> WTParam(value)
                    else -> WTParam(value.toString())
                }
            }
        }
        val screen = WTScreen(
            name = name,
            displayName = displayName,
            type = screenType,
            params = paramsMap,
            trigger = trigger,
        )
        screen.isAuto = jsonObject.optBoolean("is_auto") == true

        WiseTrack.trackScreen(screen)
    }

    @JvmStatic
    fun getAdId(): String? = WiseTrack.getADID()

    @JvmStatic
    fun getReferrer(): String? = WiseTrack.getReferrer()

    @JvmStatic
    fun getLastDeeplink(): String? = WiseTrack.getLastDeeplink()

    @JvmStatic
    fun getDeferredDeeplink(): String? = WiseTrack.getDeferredDeeplink()
}

private fun JSONObject.toMap(): Map<String, String> {
    val map = mutableMapOf<String, String>()
    val keys = this.keys()
    while (keys.hasNext()) {
        val key = keys.next()
        val value = this.get(key)
        map[key] = value.toString()
    }
    return map
}

internal object UnityLoggerOutput : WTLoggerOutput {
    private const val UNITY_GAME_OBJECT = "WiseTrackManager"
    private const val UNITY_CALLBACK_METHOD = "OnLogReceived"
    override fun log(level: WTLogLevel, tag: String, message: String, throwable: Throwable?) {
        try {
            val stackTrace = throwable?.stackTraceToString()

            // Send to Unity main thread
            UnityPlayer.UnitySendMessage(
                UNITY_GAME_OBJECT,
                UNITY_CALLBACK_METHOD,
                "${level.priority}|$tag|$message|${stackTrace ?: ""}"
            )
        } catch (e: Exception) {
            Log.e("WiseTrackUnity", "Failed to send log to Unity: ${e.message}")
        }
    }
}

internal object UnityDeepLinkReceiver {
    private const val UNITY_GAME_OBJECT = "WiseTrackManager"
    private const val UNITY_CALLBACK_METHOD = "OnDeepLinkReceived"

    fun onDeeplinkReceived(intent: Intent, isDeferred: Boolean) {
        try {
            val url = intent.dataString ?: return
            // Format: "url|isDeferred" - Unity will parse this
            val message = "$url|$isDeferred"

            // Send to Unity main thread
            UnityPlayer.UnitySendMessage(
                UNITY_GAME_OBJECT,
                UNITY_CALLBACK_METHOD,
                message
            )
        } catch (e: Exception) {
            Log.e("WiseTrackUnity", "Failed to send deeplink to Unity: ${e.message}")
        }
    }
}