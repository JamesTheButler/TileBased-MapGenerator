namespace Convenience.Geometry {
    public class Point2DInt {
        public Point2DInt(int x, int y) {
            X = x;
            Y = y;
        }

        public int X, Y;

        public override string ToString() {
            return $"[{X}, {Y}]";
        }

        public override bool Equals(object obj) {
            return obj is Point2DInt point && X == point.X && Y == point.Y;
        }

        public override int GetHashCode() {
            return 100000 * X + Y;
        }
    }
}
