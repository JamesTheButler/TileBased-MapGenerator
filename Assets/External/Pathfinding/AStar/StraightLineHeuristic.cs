using Convenience.Geometry;
using System;

namespace Pathfinding.AStar {
    public class StraightLineHeuristic : Heuristic {
        public override double Calculate(Point2D position, Point2D destination) {
            return Math.Sqrt(Math.Pow(position.X - destination.X, 2) + Math.Pow(position.Y - destination.Y, 2));
        }
    }
}
