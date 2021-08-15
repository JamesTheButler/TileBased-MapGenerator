using Convenience.Geometry;
using UnityEngine;

public static class Vector2IntExtensions {
    public static Point2D ToPoint(this Vector2Int vector) {
        return new Point2D(vector.x, vector.y);
    }
}