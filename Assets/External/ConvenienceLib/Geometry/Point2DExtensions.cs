namespace Convenience.Geometry {
    public static class Point2DExtensions {
        public static bool IsInside(this Point2D point, Point2D min, Point2D max) {
            return point.X >= min.X && point.X < max.X && point.Y >= min.Y && point.Y < max.Y;
        }
    }
}
