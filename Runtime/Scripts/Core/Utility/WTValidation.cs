using System.Collections.Generic;

/// <summary>
/// Lightweight client-side checks that mirror the native SDK limits.
/// The native SDK stays the source of truth (it rejects invalid data); these checks only surface
/// problems early in the Unity console.
/// </summary>
internal static class WTValidation
{
    internal const int EventNameMaxLength = 50;
    internal const int ScreenNameMaxLength = 150;
    internal const int ParamKeyMaxLength = 50;
    internal const int ParamValueMaxLength = 100;

    /// <summary>Returns the list of problems found in <paramref name="config"/>; empty when valid.</summary>
    internal static List<string> ValidateConfig(WTInitialConfig config)
    {
        var errors = new List<string>();
        if (config == null)
        {
            errors.Add("config is null");
            return errors;
        }
        if (string.IsNullOrEmpty(config.AppToken)) errors.Add("AppToken is required");
        if (string.IsNullOrEmpty(config.ClientSecret)) errors.Add("ClientSecret is required");
        if (config.TrackingWaitingTime < 0) errors.Add("TrackingWaitingTime must not be negative");
        return errors;
    }

    internal static List<string> ValidateEvent(WTEvent wtEvent)
    {
        var errors = new List<string>();
        if (wtEvent == null)
        {
            errors.Add("event is null");
            return errors;
        }
        ValidateName(wtEvent.Name, EventNameMaxLength, errors);
        ValidateParams(wtEvent.Params, errors);
        return errors;
    }

    internal static List<string> ValidateScreen(WTScreen screen)
    {
        var errors = new List<string>();
        if (screen == null)
        {
            errors.Add("screen is null");
            return errors;
        }
        ValidateName(screen.Name, ScreenNameMaxLength, errors);
        ValidateParams(screen.Params, errors);
        return errors;
    }

    private static void ValidateName(string name, int maxLength, List<string> errors)
    {
        if (string.IsNullOrEmpty(name))
            errors.Add("name must be a non-empty string");
        else if (name.Length > maxLength)
            errors.Add("name '" + name + "' exceeds " + maxLength + " characters");
    }

    private static void ValidateParams(Dictionary<string, WTParamValue> parameters, List<string> errors)
    {
        if (parameters == null) return;
        foreach (var kv in parameters)
        {
            if (string.IsNullOrEmpty(kv.Key))
            {
                errors.Add("parameter key must be a non-empty string");
                continue;
            }
            if (kv.Key.Length > ParamKeyMaxLength)
                errors.Add("parameter key '" + kv.Key + "' exceeds " + ParamKeyMaxLength + " characters");

            var str = kv.Value != null ? kv.Value.Value as string : null;
            if (str != null && str.Length > ParamValueMaxLength)
                errors.Add("value of parameter '" + kv.Key + "' exceeds " + ParamValueMaxLength + " characters");
        }
    }
}
