using Convenience.Geometry;

namespace Pathfinding.AStar {
    public abstract class Heuristic {
        public abstract double Calculate(Point2D position, Point2D destination);
    }
}
