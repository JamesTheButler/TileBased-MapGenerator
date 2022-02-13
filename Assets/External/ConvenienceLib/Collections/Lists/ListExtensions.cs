using System;
using System.Collections.Generic;
using System.Linq;

namespace Convenience.Collections.Lists {
    //TODO: adjust seeding
    public static class ListExtensions {
        // TODO move this to Array1DExtension
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

        //TODO: add to lib
        public static T PopFirst<T>(this List<T> list) {
            var first = list.First();
            list.RemoveAt(0);
            return first;
        }

        //TODO: add to lib
        public static T PopLast<T>(this List<T> list) {
            var last = list.Last();
            list.RemoveAt(list.Count - 1);
            return last;
        }

        //TODO: update in lib
        public static string AsString<T>(this List<T> list) {
            var s = $"List<{typeof(T)}>:[\n";
            foreach (var listElement in list) {
                s += $"{listElement}\n";
            }
            s += "]";
            return s;
        }
    }
}
