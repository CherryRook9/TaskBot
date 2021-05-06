using System.Collections.Generic;
using System.Linq;

namespace SimpleTgBot
{
    static class Extencions
    {
        public static string Serialize(this IEnumerable<KeyValuePair<string, string>> kv)
        {
            var x = string.Join(":", kv.Select(x => $"{x.Key}={x.Value}"));
            return x;
        }

        public static Dictionary<string, string> Deserialize(this string serializedKv)
        {
            var x = serializedKv
                .Split(':')
                .Select(x => {
                    var kv = x.Split('=');
                    return (kv[0], kv[1]);
                })
                .ToDictionary(x => x.Item1, x => x.Item2);

            return x;
        }
    }
}