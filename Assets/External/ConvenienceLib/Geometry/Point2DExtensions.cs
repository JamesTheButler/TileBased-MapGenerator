namespace Convenience.Geometry {
    public static class Point2DExtensions {
        public static bool IsInside(this Point2DInt point, Point2DInt min, Point2DInt max) {
            return point.X >= min.X && point.X < max.X && point.Y >= min.Y && point.Y < max.Y;
        }
    }
}
