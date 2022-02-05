using System;
using Convenience.Geometry;

namespace Pathfinding.AStar {
    public class ManhattanHeuristic : Heuristic {
        public float Calculate(Point2D position, Point2D destination) {
            return Math.Abs(position.X - destination.X) + Math.Abs(position.Y - destination.Y);
        }
    }
}
