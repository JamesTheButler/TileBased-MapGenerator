using Convenience.Geometry;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensions {
    //TODO: update name in lib
    public static Point2D ToPoint2D(this Vector2Int vector) {
        return new Point2D(vector.x, vector.y);
    }

    // TODO: add in lib
    public static Vector2Int ToVector2Int(this Point2D point) {
        return new Vector2Int(point.X, point.Y);
    }
}