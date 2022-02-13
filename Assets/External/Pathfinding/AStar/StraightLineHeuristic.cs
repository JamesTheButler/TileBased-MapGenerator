using Convenience.Geometry;
using UnityEngine;

namespace Pathfinding.AStar {
    public class StraightLineHeuristic : Heuristic {
        public float Calculate(Point2DInt position, Point2DInt destination) {
            return Mathf.Sqrt(Mathf.Pow(position.X - destination.X, 2) + Mathf.Pow(position.Y - destination.Y, 2));
        }
    }
}
