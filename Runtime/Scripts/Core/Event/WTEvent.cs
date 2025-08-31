using System;
using System.Collections.Generic;

namespace WiseTrack.Runtime
{
    /// <summary>
    /// Defines the types of events that can be tracked by WiseTrack analytics.
    /// </summary>
    public enum WTEventType
    {
        /// <summary>
        /// A standard analytics event without revenue tracking.
        /// </summary>
        Default,
        
        /// <summary>
        /// A revenue-specific event that includes monetary amount and currency information.
        /// </summary>
        Revenue
    }

    /// <summary>
    /// Represents an analytics event in the WiseTrack system.
    /// Supports both standard events and revenue events with associated parameters.
    /// </summary>
    [Serializable]
    public class WTEvent
    {
        /// <summary>
        /// Gets the type of this event (Default or Revenue).
        /// </summary>
        /// <value>The event type.</value>
        public WTEventType Type { get; }
        
        /// <summary>
        /// Gets the name of this event.
        /// </summary>
        /// <value>A string representing the event name.</value>
        public string Name { get; }
        
        /// <summary>
        /// Gets the dictionary of parameters associated with this event.
        /// Parameter keys and string values have a maximum length of 50 characters.
        /// </summary>
        /// <value>A dictionary containing event parameters, or null if no parameters are set.</value>
        public Dictionary<string, WTParamValue> Params { get; }
        
        /// <summary>
        /// Gets the revenue amount for revenue events.
        /// </summary>
        /// <value>The revenue amount, or null for non-revenue events.</value>
        public double? RevenueAmount { get; }
        
        /// <summary>
        /// Gets the revenue currency for revenue events.
        /// </summary>
        /// <value>The currency type, or null for non-revenue events.</value>
        public WTRevenueCurrency? RevenueCurrency { get; }

        /// <summary>
        /// Initializes a new instance of the WTEvent class.
        /// </summary>
        /// <param name="type">The type of event to create.</param>
        /// <param name="name">The name of the event.</param>
        /// <param name="parameters">Optional dictionary of event parameters. Keys and string values must be 50 characters or less.</param>
        /// <param name="revenueAmount">The revenue amount for revenue events.</param>
        /// <param name="revenueCurrency">The currency for revenue events.</param>
        /// <exception cref="ArgumentException">Thrown when creating a revenue event without both amount and currency.</exception>
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

        /// <summary>
        /// Creates a new default event with the specified name and optional parameters.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="parameters">Optional dictionary of event parameters. Keys and string values must be 50 characters or less.</param>
        /// <returns>A new WTEvent of type Default.</returns>
        public static WTEvent DefaultEvent(string name, Dictionary<string, WTParamValue> parameters = null)
        {
            return new WTEvent(WTEventType.Default, name, parameters);
        }

        /// <summary>
        /// Creates a new revenue event with the specified name, currency, amount, and optional parameters.
        /// </summary>
        /// <param name="name">The name of the revenue event.</param>
        /// <param name="currency">The currency type for the revenue.</param>
        /// <param name="amount">The revenue amount.</param>
        /// <param name="parameters">Optional dictionary of event parameters. Keys and string values must be 50 characters or less.</param>
        /// <returns>A new WTEvent of type Revenue.</returns>
        public static WTEvent RevenueEvent(string name, WTRevenueCurrency currency, double amount, Dictionary<string, WTParamValue> parameters = null)
        {
            return new WTEvent(WTEventType.Revenue, name, parameters, amount, currency);
        }

        /// <summary>
        /// Converts the event to a dictionary representation suitable for serialization.
        /// </summary>
        /// <returns>A dictionary containing all event data including type, name, parameters, and revenue information if applicable.</returns>
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