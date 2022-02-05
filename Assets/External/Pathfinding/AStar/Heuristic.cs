using Convenience.Geometry;

namespace Pathfinding.AStar {
    public interface Heuristic {
        float Calculate(Point2D position, Point2D destination);
    }
}
