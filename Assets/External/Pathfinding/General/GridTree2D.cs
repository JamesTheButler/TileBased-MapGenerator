using Convenience.Geometry;
using System.Collections.Generic;

namespace Pathfinding.General {
    public class GridTree2D {
        public Node[,] Nodes { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public GridTree2D(int width = 0, int height = 0) {
            Width = width;
            Height = height;
            Nodes = new Node[Width, Height];
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Nodes[x, y] = new Node(x, y, 0);
                }
            }
        }

        public GridTree2D(float[,] nodeCosts) {
            Width = nodeCosts.GetLength(0);
            Height = nodeCosts.GetLength(1);
            Nodes = new Node[Width, Height];
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Node node = new Node(x, y, nodeCosts[x, y]);
                    Nodes[x, y] = node;
                }
            }
            GenerateEdges();
        }

        public GridTree2D(GridTree2D map) {
            Width = map.Width;
            Height = map.Height;
            Nodes = new Node[Width, Height];
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    var copiedNode = map.Nodes[x, y];
                    Nodes[x, y] = new Node(copiedNode.Coordinates.X, copiedNode.Coordinates.Y, copiedNode.Cost);
                }
            }
            GenerateEdges();
        }

        /// <summary>
        /// Generate edges between nodes.
        /// </summary>
        private void GenerateEdges() {
            // steps to adjacent cells
            List<Point2D> adjSteps = new List<Point2D> {
                new Point2D(-1, 0),
                new Point2D(1, 0),
                new Point2D(0, -1),
                new Point2D(0, 1)
            };

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    Point2D pos = new Point2D(x, y);
                    // find neighbours
                    foreach (var adjStep in adjSteps) {
                        // check if neighbour is on map
                        Point2D adjPos = new Point2D(pos.X + adjStep.X, pos.Y + adjStep.Y);

                        if (adjPos.IsInside(new Point2D(0, 0), new Point2D(Width, Height))) {
                            // add edge to both nodes, if no edge exists between the nodes
                            Node currNode = Nodes[x, y];
                            Node adjNode = Nodes[adjPos.X, adjPos.Y];
                            if (!currNode.IsConnected(adjNode) && currNode.Cost != -1 && adjNode.Cost != -1) {
                                var edge = new Edge((currNode.Cost + adjNode.Cost) / 2.0f, currNode, adjNode);
                                currNode.Edges.Add(edge);
                                adjNode.Edges.Add(edge);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates all node costs.
        /// </summary>
        public void Update(float[,] newCosts) {
            if (newCosts.GetLength(0) != Nodes.GetLength(0) || newCosts.GetLength(1) != Nodes.GetLength(1))
                return;

            for (int i = 0; i < Nodes.GetLength(0); i++) {
                for (int j = 0; j < Nodes.GetLength(1); j++) {
                    Nodes[i, j].Cost = newCosts[i, j];
                }
            }
        }

        /// <summary>
        /// Returns an array of the nodes costs.
        /// </summary>
        public float[,] GetCostField() {
            var costs = new float[Nodes.GetLength(0), Nodes.GetLength(1)];
            for (int i = 0; i < costs.GetLength(0); i++) {
                for (int j = 0; j < costs.GetLength(1); j++) {
                    costs[i, j] = Nodes[i, j].Cost;
                }
            }
            return costs;
        }

        public override string ToString() {
            string s = "";
            for (int i = 0; i < Nodes.GetLength(1); i++) {
                for (int j = 0; j < Nodes.GetLength(0); j++) {
                    s += Nodes[j, i].Cost + "\t";
                }
                s += "\n";
            }
            return s;
        }
    }
}
