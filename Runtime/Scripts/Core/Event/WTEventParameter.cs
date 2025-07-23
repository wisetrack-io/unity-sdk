using System;

namespace WiseTrack.Runtime
{
    [Serializable]
    public class WTEventParam
    {
        public string Key { get; set; }
        public WTParamValue Value { get; set; }
    }

    public class WTParamValue
    {
        public object Value { get; }

        private WTParamValue(object value)
        {
            Value = value;
        }

        public static WTParamValue FromString(string value) => new(value);

        public static WTParamValue FromNumber(double value) => new(value);

        public static WTParamValue FromBoolean(bool value) => new(value);

        public static WTParamValue FromDynamic(object value)
        {
            if (value is string || value is double || value is float || value is int || value is decimal || value is long || value is bool)
            {
                return new WTParamValue(value);
            }

            throw new Exception($"Invalid value for WTParamValue: `{value}`");
        }

        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}