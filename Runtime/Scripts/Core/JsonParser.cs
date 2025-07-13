using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WiseTrack.Core
{
    public static class JsonParser
    {
        public static string Serialize(object obj)
        {
            if (obj == null) return null;
            if (obj is string str) return $"\"{str}\"";
            if (obj is bool b) return b ? "true" : "false";
            if (obj is IDictionary<string, object> dict)
            {
                var items = new List<string>();
                foreach (var kv in dict)
                {
                    if (kv.Value != null)
                        items.Add($"\"{kv.Key}\":{Serialize(kv.Value)}");
                }
                return "{" + string.Join(",", items) + "}";
            }
            if (obj is IEnumerable list)
            {
                var items = new List<string>();
                foreach (var i in list)
                    items.Add(Serialize(i));
                return "[" + string.Join(",", items) + "]";
            }
            return obj.ToString();
        }
    }
}