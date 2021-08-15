namespace Convenience.Geometry {
    public class Point2D {
        public Point2D(int x, int y) {
            X = x;
            Y = y;
        }

        public int X, Y;

        public override string ToString() {
            return $"[{X}, {Y}]";
        }
    }
}
