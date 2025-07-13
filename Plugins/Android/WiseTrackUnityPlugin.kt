package io.wisetrack.sdk.unity

import android.app.Activity
import android.util.Log
import org.json.JSONObject
import com.unity3d.player.UnityPlayer
import io.wisetrack.sdk.core.WiseTrack
import io.wisetrack.sdk.core.models.EventParam
import io.wisetrack.sdk.core.models.RevenueCurrency
import io.wisetrack.sdk.core.models.WTEvent
import io.wisetrack.sdk.core.models.WTEventType
import io.wisetrack.sdk.core.models.WTStoreName
import io.wisetrack.sdk.core.core.WTUserEnvironment
import io.wisetrack.sdk.core.models.WTInitialConfig
import io.wisetrack.sdk.core.utils.wrapper.ResourceWrapper
import io.wisetrack.sdk.core.models.WTLogLevel
import io.wisetrack.sdk.core.utils.WTLoggerOutput


object WiseTrackUnityPlugin {

    private const val TAG = "WiseTrackUnityPlugin"

    private fun getActivity(): Activity = UnityPlayer.currentActivity

    private fun getWiseTrack(): WiseTrack = WiseTrack.getInstance(getActivity())

    @JvmStatic
    fun initialize(jsonConfig: String) {
        try {
            val json = JSONObject(jsonConfig)

            val appToken = json.getString("app_token")
            val sdkVersion = json.getString("sdk_version")
            val sdkEnv = json.getString("sdk_environment")
            val userEnv = json.getString("user_environment")
            val storeName = json.optString("android_store_name", "other")
            val trackingWaitingTime = json.optInt("tracking_waiting_time", 0)
            val startTrackerAutomatically = json.optBoolean("start_tracker_automatically", true)
            val customDeviceId = if(json.has("custom_device_id")) json.getString("custom_device_id") else null
            val defaultTracker = if(json.has("default_tracker")) json.getString("default_tracker") else null
            val appSecret = if(json.has("app_secret")) json.getString("app_secret") else null
            val secretId = if(json.has("secret_id")) json.getString("secret_id") else null
            val attributionDeeplink = json.optBoolean("attribution_deeplink", false)
            val eventBuffering = json.optBoolean("event_buffering_enabled", false)
            val logLevel = json.optInt("log_level", 3)
            val oaidEnabled = json.optBoolean("oaid_enabled", false)
            val referrerEnabled = json.optBoolean("referrer_enabled", true)

           
           ResourceWrapper.setFramework("unity")
           ResourceWrapper.setSdkHash("74f6c5494c51b3339b40535a48be85190bcc4472")
           ResourceWrapper.setEnvironment(sdkEnv)
           ResourceWrapper.setVersion(sdkVersion)

           val config = WTInitialConfig(
               appToken = appToken,
               environment = WTUserEnvironment.valueOf(userEnv.uppercase()),
               storeName = WTStoreName.fromString(storeName),
               trackingWaitingTime = trackingWaitingTime,
               startTrackerAutomatically = startTrackerAutomatically,
               customDeviceId = customDeviceId,
               defaultTracker = defaultTracker,
               appSecret = appSecret,
               secretId = secretId,
               attributionDeeplink = attributionDeeplink,
               eventBuffering = eventBuffering,
               logLevel = WTLogLevel.fromPriority(logLevel),
               oaidEnabled = oaidEnabled,
               referrerEnabled = referrerEnabled
           )

           getWiseTrack().initialize(config)

        } catch (e: Exception) {
            Log.e("WiseTrackUnityPlugin", "Failed to parse config: $jsonConfig", e)
        }
    }

    @JvmStatic
    fun addLoggerOutput() {
        getWiseTrack().addLoggerOutput(UnityLoggerOutput)
    }

    @JvmStatic
    fun setLogLevel(level: Int) {
        getWiseTrack().setLogLevel(WTLogLevel.fromPriority(level))
    }

    @JvmStatic
    fun clearDataAndStop() {
        getWiseTrack().clearDataAndStop()
    }

    @JvmStatic
    fun setEnabled(enabled: Boolean) {
        getWiseTrack().setEnabled(enabled)
    }

    @JvmStatic
    fun isEnabled(): Boolean {
        return getWiseTrack().isEnabled
    }

    @JvmStatic
    fun startTracking() {
        getWiseTrack().startTracking()
    }

    @JvmStatic
    fun stopTracking() {
        getWiseTrack().stopTracking()
    }

    @JvmStatic
    fun setFCMToken(token: String?) {
        if (!token.isNullOrEmpty()) {
            getWiseTrack().setFCMToken(token)
        }
    }

    @JvmStatic
    fun logEvent(json: String) {
        val jsonObject = JSONObject(json)
        val name = jsonObject.getString("name")
        val type = jsonObject.getString("type")
        val revenue = jsonObject.optDouble("revenue", 0.0)
        val currency = jsonObject.optString("currency", "USD")

        val paramsJson = jsonObject.optJSONObject("params")
        val paramsMap = mutableMapOf<String, EventParam>()

        if (paramsJson != null) {
            for (key in paramsJson.keys()){
                paramsMap[key] = when (val value = paramsJson.get(key)) {
                    is Int -> EventParam(value.toDouble())
                    is Double -> EventParam(value)
                    is Boolean -> EventParam(value)
                    else -> EventParam(value.toString())
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

        getWiseTrack().logEvent(event)
    }


    @JvmStatic
    fun getAdId(): String? = getWiseTrack().getADID()

    @JvmStatic
    fun getReferrer(): String? = getWiseTrack().getReferrer()
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