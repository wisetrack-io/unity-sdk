using System;
using System.Collections.Generic;

public enum WTScreenType
{
    Page,
    Dialog,
    BottomSheet,
    Other
}

[Serializable]
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
            { "type", Type.ToString().ToLower() },
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
                paramMap[kvp.Key] = kvp.Value.Value;
            }
            map["params"] = paramMap;
        }

        return map;
    }
}
