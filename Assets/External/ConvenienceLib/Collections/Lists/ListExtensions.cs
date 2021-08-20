using System;
using System.Collections.Generic;
using System.Linq;

namespace Convenience.Collections.Lists {
    //TODO: adjust seeding
    public static class ListExtensions {
        // TODO move this to Array1DExteionsn
        // TODO keep same format
        /// <summary>
        /// Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this T[] array) {
            array.Shuffle(new Random());
        }

        /// <summary>
        /// Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this T[] array, int seed) {
            array.Shuffle(new Random(seed));
        }

        /// <summary>
        /// Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this T[] array, Random rng) {
            int n = array.Length;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        /// <summary>
        /// Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this List<T> list) {
            list.Shuffle(new Random().Next());
        }

        /// <summary>
        /// Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this List<T> list, int seed) {
            list.Shuffle(new Random(seed));
        }

        /// <summary>
        /// Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this List<T> list, Random rng) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        //TODO: add to lib
        public static bool IsEmpty<T>(this List<T> list) {
            return list.Count <= 0;
        }

        public static string AsString<T>(this List<T> arg) {
            return arg.ToString();
        }

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
