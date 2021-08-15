using System.Collections.Generic;

namespace Graph {
    public class Node {
        public string Name { get; private set; }

        public Node(string name) {
            Name = name;
        }

        List<Edge> edges;

        public List<Node> GetNeighbors() {
            var neighbors = new List<Node>();
            foreach (var edge in edges) {
                neighbors.Add(edge.GetOther(this));
            }
            return neighbors;
        }

        public override string ToString() {
            return $"Node({Name})";
        }

        public override bool Equals(object other) {
            return (other is Node || ((Node)other).Name == Name);
        }
    }
}