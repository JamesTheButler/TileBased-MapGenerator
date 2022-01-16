using Convenience.Geometry;

namespace Pathfinding.AStar {
    public interface Heuristic {
        double Calculate(Point2D position, Point2D destination);
    }
}
