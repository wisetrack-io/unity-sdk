using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/// <summary>
/// Minimal JSON writer used to pass data to the native bridge.
/// Strings are escaped per RFC 8259 and numbers are always written with the invariant culture,
/// so the output does not depend on the device locale.
/// </summary>
internal static class JsonParser
{
    public static string Serialize(object obj)
    {
        var sb = new StringBuilder();
        WriteValue(sb, obj);
        return sb.ToString();
    }

    private static void WriteValue(StringBuilder sb, object obj)
    {
        if (obj == null)
        {
            sb.Append("null");
            return;
        }

        var str = obj as string;
        if (str != null)
        {
            WriteString(sb, str);
            return;
        }

        if (obj is bool)
        {
            sb.Append((bool)obj ? "true" : "false");
            return;
        }

        if (obj is char)
        {
            WriteString(sb, obj.ToString());
            return;
        }

        if (obj is Enum)
        {
            WriteString(sb, obj.ToString());
            return;
        }

        if (obj is double || obj is float)
        {
            var d = Convert.ToDouble(obj, CultureInfo.InvariantCulture);
            if (double.IsNaN(d) || double.IsInfinity(d))
                sb.Append("null");
            else
                sb.Append(d.ToString("R", CultureInfo.InvariantCulture));
            return;
        }

        if (obj is decimal || obj is int || obj is long || obj is short || obj is byte ||
            obj is uint || obj is ulong || obj is ushort || obj is sbyte)
        {
            sb.Append(((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture));
            return;
        }

        var dict = obj as IDictionary<string, object>;
        if (dict != null)
        {
            WriteObject(sb, dict);
            return;
        }

        var stringDict = obj as IDictionary<string, string>;
        if (stringDict != null)
        {
            var copy = new Dictionary<string, object>();
            foreach (var kv in stringDict) copy[kv.Key] = kv.Value;
            WriteObject(sb, copy);
            return;
        }

        var list = obj as IEnumerable;
        if (list != null)
        {
            sb.Append('[');
            var first = true;
            foreach (var item in list)
            {
                if (!first) sb.Append(',');
                first = false;
                WriteValue(sb, item);
            }
            sb.Append(']');
            return;
        }

        WriteString(sb, obj.ToString());
    }

    private static void WriteObject(StringBuilder sb, IDictionary<string, object> dict)
    {
        sb.Append('{');
        var first = true;
        foreach (var kv in dict)
        {
            // Null values are omitted so the native side can treat them as "not set".
            if (kv.Key == null || kv.Value == null) continue;
            if (!first) sb.Append(',');
            first = false;
            WriteString(sb, kv.Key);
            sb.Append(':');
            WriteValue(sb, kv.Value);
        }
        sb.Append('}');
    }

    private static void WriteString(StringBuilder sb, string s)
    {
        sb.Append('"');
        foreach (var c in s)
        {
            switch (c)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                case '\b': sb.Append("\\b"); break;
                case '\f': sb.Append("\\f"); break;
                case '\u2028': sb.Append("\\u2028"); break;
                case '\u2029': sb.Append("\\u2029"); break;
                default:
                    if (c < 0x20)
                        sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                    else
                        sb.Append(c);
                    break;
            }
        }
        sb.Append('"');
    }
}
