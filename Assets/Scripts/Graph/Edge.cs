using System;

namespace Graph {
    public class Edge {
        public float Cost { get; set; }
        public Tuple<Node, Node> Nodes { get; private set; }

        public Edge(float cost, Tuple<Node, Node> nodes) {
            Cost = cost;
            Nodes = nodes;
        }

        public Edge(float cost, Node node1, Node node2) : this(cost, new Tuple<Node, Node>(node1, node2)) { }

        public Node GetOther(Node node) {
            return Nodes.Item1 == node ? Nodes.Item2 : Nodes.Item1;
        }
    }
}