using System.Collections.Generic;

namespace Convenience.Collections.Dictionary {

    public static class DictionaryExtensions {
        public static string AsString<K, V>(this Dictionary<K, V> dict) {
            var s = $"Dictionary<{typeof(K)},{typeof(V)}>:[\n";
            foreach (var kvpair in dict) {
                s += $"{kvpair.Key}:{kvpair.Value}\n";
            }
            s += "]";
            return s;
        }
    }
}