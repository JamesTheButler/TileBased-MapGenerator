using System;

namespace Convenience {
    public static class Math {
        public static T Clamp<T>(T value, T min, T max) where T : IComparable {
            return value.CompareTo(min) < 0 ? min : (value.CompareTo(max) > 0 ? max : value);
        }
    }
}
