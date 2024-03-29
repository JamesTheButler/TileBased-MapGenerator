using Convenience.Geometry;
using UnityEngine;

public static class Vector2IntExtensions {
    //TODO: update name in lib
    public static Point2DInt ToPoint2D(this Vector2Int vector) {
        return new Point2DInt(vector.x, vector.y);
    }

    // TODO: add in lib
    public static Vector2Int ToVector2Int(this Point2DInt point) {
        return new Vector2Int(point.X, point.Y);
    }

    /* Returns true if x and y of this vector are > 0 and < other*/
    public static bool IsInside(this Vector2Int thisVector, Vector2Int other) {
        return thisVector.x >= 0 && thisVector.y >= 0 && thisVector.x < other.x && thisVector.y < other.y;
    }
}