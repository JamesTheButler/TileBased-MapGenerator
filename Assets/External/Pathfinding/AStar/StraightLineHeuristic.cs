using Convenience.Geometry;
using UnityEngine;

namespace Pathfinding.AStar {
    public class StraightLineHeuristic : Heuristic {
        public float Calculate(Point2D position, Point2D destination) {
            return Mathf.Sqrt(Mathf.Pow(position.X - destination.X, 2) + Mathf.Pow(position.Y - destination.Y, 2));
        }
    }
}
