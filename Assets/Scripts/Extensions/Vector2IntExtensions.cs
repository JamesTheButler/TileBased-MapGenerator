using System;
using UnityEngine;

public static class Vector2IntExtensions {
    public static bool IsInside(this Vector2Int v, Vector2Int min, Vector2Int max) {
        return (v.x >= min.x) && (v.x < max.x) && (v.y >= min.y) && (v.y < max.y);
    }
}
