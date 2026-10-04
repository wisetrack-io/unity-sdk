using System.Collections.Generic;

/// <summary>
/// Presentation style of a tracked screen.
/// </summary>
public enum WTScreenType
{
    Page,
    Dialog,
    BottomSheet,
    Other
}

/// <summary>
/// Represents a screen (scene, UI panel, menu, dialog, ...) tracked by WiseTrack.
/// The screen name must be 1-150 characters; parameter keys must be 1-50 characters and
/// string parameter values at most 100 characters.
/// </summary>
public class WTScreen
{
    public WTScreenType Type { get; }
    public string Name { get; }
    public string DisplayName { get; }
    public Dictionary<string, WTParamValue> Params { get; }
    internal bool IsAuto { get; }
    internal string Trigger { get; }

    public WTScreen(
        string name,
        WTScreenType type = WTScreenType.Other,
        string displayName = null,
        Dictionary<string, WTParamValue> parameters = null,
        bool isAuto = false,
        string trigger = null)
    {
        Name = name;
        Type = type;
        DisplayName = displayName;
        Params = parameters;
        IsAuto = isAuto;
        Trigger = trigger;
    }

    public Dictionary<string, object> ToMap()
    {
        var map = new Dictionary<string, object>
        {
            { "type", ToWireName(Type) },
            { "name", Name },
            { "display_name", DisplayName },
            { "trigger", Trigger },
            { "is_auto", IsAuto },
        };

        if (Params != null)
        {
            var paramMap = new Dictionary<string, object>();
            foreach (var kvp in Params)
            {
                paramMap[kvp.Key] = kvp.Value != null ? kvp.Value.Value : null;
            }
            map["params"] = paramMap;
        }

        return map;
    }

    /// <summary>
    /// Maps the screen type to the identifier used by the native SDK (<c>WTScreenType.sName</c>).
    /// </summary>
    internal static string ToWireName(WTScreenType type)
    {
        switch (type)
        {
            case WTScreenType.Page: return "page";
            case WTScreenType.Dialog: return "dialog";
            case WTScreenType.BottomSheet: return "bottom_sheet";
            default: return "other";
        }
    }
}
