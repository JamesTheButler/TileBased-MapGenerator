using System;
using Convenience.Geometry;

namespace Pathfinding.AStar {
    public class ManhattanHeuristic : Heuristic {
        public float Calculate(Point2DInt position, Point2DInt destination) {
            return Math.Abs(position.X - destination.X) + Math.Abs(position.Y - destination.Y);
        }
    }
}
