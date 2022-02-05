using System;

namespace Pathfinding.General {
    public class Edge {
        public float Cost { get; set; }
        public Tuple<Node, Node> Nodes { get; set; }

        public Edge(float cost, Tuple<Node, Node> nodes) {
            Cost = cost;
            Nodes = nodes;
        }

        public Edge(float cost, Node node1, Node node2) : this(cost, new Tuple<Node, Node>(node1, node2)) { }

        public override string ToString() {
            return $"{Nodes.Item1}-{Nodes.Item2}";
        }
    }
}
