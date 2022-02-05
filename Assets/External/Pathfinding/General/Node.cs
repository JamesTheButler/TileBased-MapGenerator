using Convenience.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pathfinding.General {
    public class Node {
        public List<Edge> Edges { get; set; }
        public Point2D Coordinates { get; private set; }
        public float Cost { get; set; }
        public Node NearestNodeToStart { get; set; }
        public float MinCostToStart { get; set; }
        public bool WasVisited { get; set; }
        public Guid Id { get; set; }

        public Node() {
            NearestNodeToStart = null;
            MinCostToStart = float.MaxValue;
            Id = Guid.NewGuid();
            Edges = new List<Edge>();
            Coordinates = new Point2D(-1, -1);
        }

        public Node(int x, int y, float cost = 0) : this() {
            Coordinates = new Point2D(x, y);
            Cost = cost;
        }

        public bool IsConnected(Node connNode) {
            foreach (var edge in Edges) {
                if (edge.Nodes.Item1.Equals(connNode) || edge.Nodes.Item2.Equals(connNode)) {
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj) {
            if (obj == null)
                return false;
            if (!(obj is Node node))
                return false;
            return node.Id == Id;
        }

        public override string ToString() {
            return $"[{Coordinates.X},\t {Coordinates.Y}] {Cost}";
        }
    }
}
