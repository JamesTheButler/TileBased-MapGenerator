using Convenience.Geometry;
using System;
using System.Collections.Generic;

namespace Pathfinding.General {
    public class Node {
        public List<Edge> Edges { get; set; }
        public Point2D Coordinates { get; protected set; }
        public float Cost { get; set; }

        public Node() {
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
            return obj is Node node && Coordinates == node.Coordinates;
        }

        public override string ToString() {
            return $"[{Coordinates.X},\t {Coordinates.Y}] {Cost}";
        }

        public override int GetHashCode() {
            return Coordinates.GetHashCode();
        }
    }
}
