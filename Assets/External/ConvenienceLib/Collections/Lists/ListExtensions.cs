using System;

namespace Convenience.Collections.Lists {
    public static class ListExtensions {
        /// <summary>
        /// Fisher-Yates shuffle.
        /// </summary>
        public static void Shuffle<T>(this T[] array) {
            array.Shuffle(new Random());
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
    }
}
