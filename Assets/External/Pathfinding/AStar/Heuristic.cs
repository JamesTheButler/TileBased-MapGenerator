using Convenience.Geometry;

namespace Pathfinding.AStar {
    public interface Heuristic {
        float Calculate(Point2DInt position, Point2DInt destination);
    }
}
