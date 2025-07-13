using System;
using System.Collections.Generic;

namespace WiseTrack.Runtime
{
    public enum WTEventType
    {
        Default,
        Revenue
    }

    [Serializable]
    public class WTEvent
    {
        public WTEventType Type { get; }
        public string Name { get; }
        public Dictionary<string, WTParamValue> Params { get; }
        public double? RevenueAmount { get; }
        public WTRevenueCurrency? RevenueCurrency { get; }

        private WTEvent(
            WTEventType type,
            string name,
            Dictionary<string, WTParamValue> parameters = null,
            double? revenueAmount = null,
            WTRevenueCurrency? revenueCurrency = null)
        {
            if (type == WTEventType.Revenue)
            {
                if (revenueAmount == null || revenueCurrency == null)
                    throw new ArgumentException("For revenue events, both revenueAmount and revenueCurrency must be provided.");
            }

            Type = type;
            Name = name;
            Params = parameters;
            RevenueAmount = revenueAmount;
            RevenueCurrency = revenueCurrency;
        }

        public static WTEvent DefaultEvent(string name, Dictionary<string, WTParamValue> parameters = null)
        {
            return new WTEvent(WTEventType.Default, name, parameters);
        }

        public static WTEvent RevenueEvent(string name, WTRevenueCurrency currency, double amount, Dictionary<string, WTParamValue> parameters = null)
        {
            return new WTEvent(WTEventType.Revenue, name, parameters, amount, currency);
        }

        public Dictionary<string, object> ToMap()
        {
            var map = new Dictionary<string, object>
        {
            { "type", Type.ToString().ToLower() },
            { "name", Name }
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

            if (Type == WTEventType.Revenue)
            {
                map["revenue"] = RevenueAmount;
                map["currency"] = RevenueCurrency.ToString();
            }

            return map;
        }
    }
}