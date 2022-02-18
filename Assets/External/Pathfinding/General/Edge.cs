using System;

namespace Pathfinding.General {
    public class Edge {
        public Tuple<Node, Node> Nodes { get; set; }

        public Edge(Tuple<Node, Node> nodes) {
            Nodes = nodes;
        }

        public Edge(Node node1, Node node2) : this(new Tuple<Node, Node>(node1, node2)) { }

        public override string ToString() {
            return $"{Nodes.Item1}-{Nodes.Item2}";
        }
    }
}
