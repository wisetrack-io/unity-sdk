using System;

namespace WiseTrack.Runtime
{
    /// <summary>
    /// Represents an event parameter with a key-value pair for WiseTrack analytics events.
    /// </summary>
    [Serializable]
    public class WTEventParam
    {
        /// <summary>
        /// Gets or sets the parameter key/name that identifies this event parameter.
        /// Maximum length is 50 characters.
        /// </summary>
        /// <value>A string representing the parameter key.</value>
        public string Key { get; set; }
        
        /// <summary>
        /// Gets or sets the parameter value wrapped in a WTParamValue object.
        /// </summary>
        /// <value>A WTParamValue containing the actual parameter value.</value>
        public WTParamValue Value { get; set; }
    }

    /// <summary>
    /// Represents a type-safe wrapper for event parameter values in WiseTrack.
    /// Supports string, numeric, and boolean values with validation and type conversion.
    /// String values have a maximum length of 50 characters.
    /// </summary>
    public class WTParamValue
    {
        /// <summary>
        /// Gets the underlying value stored in this parameter wrapper.
        /// </summary>
        /// <value>The raw object value that was wrapped.</value>
        public object Value { get; }

        /// <summary>
        /// Initializes a new instance of the WTParamValue class with the specified value.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        private WTParamValue(object value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new WTParamValue instance from a string value.
        /// </summary>
        /// <param name="value">The string value to wrap. Maximum length is 50 characters.</param>
        /// <returns>A new WTParamValue containing the string value.</returns>
        public static WTParamValue FromString(string value) => new(value);

        /// <summary>
        /// Creates a new WTParamValue instance from a numeric value.
        /// </summary>
        /// <param name="value">The double value to wrap.</param>
        /// <returns>A new WTParamValue containing the numeric value.</returns>
        public static WTParamValue FromNumber(double value) => new(value);

        /// <summary>
        /// Creates a new WTParamValue instance from a boolean value.
        /// </summary>
        /// <param name="value">The boolean value to wrap.</param>
        /// <returns>A new WTParamValue containing the boolean value.</returns>
        public static WTParamValue FromBoolean(bool value) => new(value);

        /// <summary>
        /// Creates a new WTParamValue instance from a dynamic object value.
        /// Validates that the value is one of the supported types before wrapping.
        /// </summary>
        /// <param name="value">The object value to wrap. Must be string, double, float, int, decimal, long, or bool.</param>
        /// <returns>A new WTParamValue containing the validated value.</returns>
        /// <exception cref="Exception">Thrown when the provided value type is not supported.</exception>
        public static WTParamValue FromDynamic(object value)
        {
            if (value is string || value is double || value is float || value is int || value is decimal || value is long || value is bool)
            {
                return new WTParamValue(value);
            }

            throw new Exception($"Invalid value for WTParamValue: `{value}`");
        }

        /// <summary>
        /// Returns a string representation of the wrapped value.
        /// </summary>
        /// <returns>A string representation of the Value, or null if Value is null.</returns>
        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}