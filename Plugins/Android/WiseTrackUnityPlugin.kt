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

/**
 * Bridge between the Unity C# layer (AndroidPlatform.cs) and the WiseTrack Android SDK.
 */
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
            val userEnv = json.getString("user_environment")
            val storeName = json.optString("android_store_name", "other")
            val trackingWaitingTime = json.optInt("tracking_waiting_time", 0)
            val startTrackerAutomatically = json.optBoolean("start_tracker_automatically", true)
            val customDeviceId = json.optStringOrNull("custom_device_id")
            val defaultTracker = json.optStringOrNull("default_tracker")
            val deeplinkEnabled = json.optBoolean("deeplink_enabled", true)
            val logLevel = json.optInt("log_level", 5)
            val oaidEnabled = json.optBoolean("oaid_enabled", false)
            val agentDiagnostics = json.optBoolean("agent_diagnostics", false)

            val activity = getActivity()

            val resourceWrapper = ResourceWrapper(activity)
            resourceWrapper.setFramework("unity")
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
            ).apply {
                this.agentDiagnostics = agentDiagnostics
            }

            // Register the deeplink listener before initialize, so a deeplink dispatched during
            // initialization is not lost.
            WiseTrack.ensureInitialized(activity)
            registerDeeplinkListener()

            WiseTrack.initialize(activity, config)
            // C# initializes after the Unity activity resumed, so the native lifecycle callbacks
            // never saw the link that launched the app: hand it over (handled once).
            activity.intent?.takeIf { it.data != null }?.let { WiseTrack.processDeeplink(it) }
        } catch (e: Exception) {
            // Never log the config itself: it contains the client secret.
            Log.e(TAG, "Failed to initialize WiseTrack: ${e.message}", e)
        }
    }

    @JvmStatic
    fun isWiseTrackNotificationPayload(jsonData: String): Boolean {
        return try {
            WiseTrack.isWiseTrackNotificationPayload(JSONObject(jsonData).toStringMap())
        } catch (e: Exception) {
            Log.e(TAG, "isWiseTrackNotificationPayload failed: ${e.message}")
            false
        }
    }

    @JvmStatic
    fun addLoggerOutput() = guard("addLoggerOutput") {
        WiseTrack.addLoggerOutput(UnityLoggerOutput)
    }

    @JvmStatic
    fun ensureInitialized() = guard("ensureInitialized") {
        WiseTrack.ensureInitialized(getActivity())
        registerDeeplinkListener()
    }

    @JvmStatic
    fun setLogLevel(level: Int) = guard("setLogLevel") {
        WiseTrack.setLogLevel(WTLogLevel.fromPriority(level))
    }

    @JvmStatic
    fun clearDataAndStop() = guard("clearDataAndStop") {
        WiseTrack.clearDataAndStop()
    }

    @JvmStatic
    fun setEnabled(enabled: Boolean) = guard("setEnabled") {
        WiseTrack.setEnabled(enabled)
    }

    @JvmStatic
    fun isEnabled(): Boolean {
        return try {
            WiseTrack.isEnabled
        } catch (e: Exception) {
            Log.e(TAG, "isEnabled failed: ${e.message}")
            false
        }
    }

    @JvmStatic
    fun startTracking() = guard("startTracking") {
        WiseTrack.startTracking()
    }

    @JvmStatic
    fun stopTracking() = guard("stopTracking") {
        WiseTrack.stopTracking()
    }

    @JvmStatic
    fun setFCMToken(token: String?) = guard("setFCMToken") {
        if (!token.isNullOrEmpty()) {
            WiseTrack.setFCMToken(token)
        }
    }

    @JvmStatic
    fun trackEvent(json: String) = guard("trackEvent") {
        val jsonObject = JSONObject(json)
        val name = jsonObject.getString("name")
        val type = jsonObject.getString("type")
        val params = parseParams(jsonObject.optJSONObject("params"))

        val event = if (type.uppercase() == WTEventType.REVENUE.name) {
            val currency = jsonObject.optString("currency", "USD")
            WTEvent.revenueEvent(
                name,
                amount = jsonObject.optDouble("revenue", 0.0),
                currency = RevenueCurrency.valueOf(currency.uppercase()),
                params = params
            )
        } else {
            WTEvent.defaultEvent(name, params = params)
        }

        WiseTrack.trackEvent(event)
    }

    @JvmStatic
    fun trackScreen(json: String) = guard("trackScreen") {
        val jsonObject = JSONObject(json)

        val screen = WTScreen(
            name = jsonObject.getString("name"),
            displayName = jsonObject.optStringOrNull("display_name"),
            type = parseScreenType(jsonObject.optStringOrNull("type")),
            params = parseParams(jsonObject.optJSONObject("params")),
            trigger = jsonObject.optStringOrNull("trigger"),
        )
        screen.isAuto = jsonObject.optBoolean("is_auto", false)

        WiseTrack.trackScreen(screen)
    }

    @JvmStatic
    fun getAdId(): String? = guardValue("getAdId") { WiseTrack.getADID() }

    @JvmStatic
    fun getReferrer(): String? = guardValue("getReferrer") { WiseTrack.getReferrer() }

    @JvmStatic
    fun getLastDeeplink(): String? = guardValue("getLastDeeplink") { WiseTrack.getLastDeeplink() }

    @JvmStatic
    fun getDeferredDeeplink(): String? = guardValue("getDeferredDeeplink") { WiseTrack.getDeferredDeeplink() }

    private fun registerDeeplinkListener() {
        WiseTrack.setOnDeeplinkListener { intent, isDeferred ->
            UnityDeepLinkReceiver.onDeeplinkReceived(intent, isDeferred)
        }
    }

    /** Maps the identifier sent by C# (WTScreen.ToWireName) to the native enum, via its `sName`. */
    private fun parseScreenType(value: String?): WTScreenType {
        if (value.isNullOrEmpty()) return WTScreenType.OTHER
        return WTScreenType.values().firstOrNull {
            it.sName.equals(value, ignoreCase = true) || it.name.equals(value, ignoreCase = true)
        } ?: WTScreenType.OTHER
    }

    private fun parseParams(paramsJson: JSONObject?): Map<String, WTParam> {
        val paramsMap = mutableMapOf<String, WTParam>()
        if (paramsJson == null) return paramsMap

        for (key in paramsJson.keys()) {
            if (paramsJson.isNull(key)) continue
            paramsMap[key] = when (val value = paramsJson.get(key)) {
                is Boolean -> WTParam(value)
                is Number -> WTParam(value.toDouble())
                else -> WTParam(value.toString())
            }
        }
        return paramsMap
    }

    private inline fun guard(method: String, block: () -> Unit) {
        try {
            block()
        } catch (e: Exception) {
            Log.e(TAG, "$method failed: ${e.message}", e)
        }
    }

    private inline fun <T> guardValue(method: String, block: () -> T?): T? {
        return try {
            block()
        } catch (e: Exception) {
            Log.e(TAG, "$method failed: ${e.message}", e)
            null
        }
    }
}

private fun JSONObject.optStringOrNull(key: String): String? =
    if (has(key) && !isNull(key)) getString(key) else null

private fun JSONObject.toStringMap(): Map<String, String> {
    val map = mutableMapOf<String, String>()
    val keys = this.keys()
    while (keys.hasNext()) {
        val key = keys.next()
        map[key] = this.get(key).toString()
    }
    return map
}

internal object UnityLoggerOutput : WTLoggerOutput {
    private const val UNITY_GAME_OBJECT = "WiseTrackManager"
    private const val UNITY_CALLBACK_METHOD = "OnLogReceived"

    override fun log(level: WTLogLevel, tag: String, message: String, throwable: Throwable?) {
        try {
            val payload = JSONObject()
                .put("level", level.priority)
                .put("tag", tag)
                .put("message", message)
            throwable?.let { payload.put("stackTrace", it.stackTraceToString()) }

            // Delivered on the Unity main thread
            UnityPlayer.UnitySendMessage(UNITY_GAME_OBJECT, UNITY_CALLBACK_METHOD, payload.toString())
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
            val payload = JSONObject()
                .put("url", url)
                .put("isDeferred", isDeferred)

            // Delivered on the Unity main thread
            UnityPlayer.UnitySendMessage(UNITY_GAME_OBJECT, UNITY_CALLBACK_METHOD, payload.toString())
        } catch (e: Exception) {
            Log.e("WiseTrackUnity", "Failed to send deeplink to Unity: ${e.message}")
        }
    }
}
