using System;

namespace Convenience.Extensions {
    public static class PrimitveExtensions {
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable {
            return Math.Clamp(value, min, max);
        }

        public static bool Not(this bool b) { return !b; }
    }
}
