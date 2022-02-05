using Convenience.Geometry;
using System.Collections.Generic;

namespace Pathfinding.General {
    public abstract class BaseSearch {
        public GridTree2D Map { get; set; }
        public Point2D StartPosition { get; set; }
        public Point2D EndPosition { get; set; }
        public List<Node> VisitedNodes { get; set; }

        public BaseSearch(GridTree2D map, Point2D startNode, Point2D endNode) {
            Map = map;
            StartPosition = startNode;
            EndPosition = endNode;
        }

        public abstract List<Node> GetPath(out float shortestPathCost);
    }
}
