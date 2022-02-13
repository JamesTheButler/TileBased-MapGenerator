using Convenience.Collections.Arrays;
using Convenience.Collections.Lists;
using Convenience.Geometry;
using Pathfinding.General;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding.AStar {
    public class AStarSearch {
        public AStarSearch(GridTree2D map, Point2DInt startNode, Point2DInt endNode, Heuristic heuristic) {
            this.map = map;
            startPosition = startNode;
            endPosition = endNode;
            this.heuristic = heuristic;

            Search();
        }

        private readonly GridTree2D map;
        private readonly Point2DInt startPosition;
        private readonly Point2DInt endPosition;
        private readonly Heuristic heuristic;

        private SearchNode[,] searchMap;

        private void Search() {
            float[,] heuristicMap = SetupHeuristics();
            //LogHeuristicMap(heuristicMap);

            searchMap = new SearchNode[map.Width, map.Height];
            for (int i = 0; i < map.Width; i++) {
                for (int j = 0; j < map.Height; j++) {
                    searchMap[i, j] = new SearchNode();
                }
            }

            Node startNode = map.Nodes[startPosition.X, startPosition.Y];
            var startSearchNode = searchMap[startPosition.X, startPosition.Y];

            startSearchNode.MinCostToStart = 0;
            var priorityQueue = new List<Node>();
            var visitedNodes = new List<Node>();
            priorityQueue.Add(startNode);

            do {
                // sort priority queue by smallest costToStart+heuristic
                priorityQueue = priorityQueue.OrderBy(priorityQNode => {
                    var priorityQSearchNode = searchMap[priorityQNode.Coordinates.X, priorityQNode.Coordinates.Y];
                    var heuristic = heuristicMap[priorityQNode.Coordinates.X, priorityQNode.Coordinates.Y];
                    return priorityQSearchNode.MinCostToStart + heuristic;
                }).ToList();

                var node = priorityQueue.PopFirst();
                var searchNode = searchMap[node.Coordinates.X, node.Coordinates.Y];
                visitedNodes.Add(node);

                foreach (var edge in node.Edges) {
                    Node otherNode;
                    SearchNode otherSearchNode;
                    if (edge.Nodes.Item1.Equals(node)) {
                        otherNode = edge.Nodes.Item2;
                    } else {
                        otherNode = edge.Nodes.Item1;
                    }
                    otherSearchNode = searchMap[otherNode.Coordinates.X, otherNode.Coordinates.Y];

                    if (otherSearchNode.WasVisited)
                        continue;

                    if (otherSearchNode.MinCostToStart > searchNode.MinCostToStart + edge.Cost) {
                        otherSearchNode.MinCostToStart = searchNode.MinCostToStart + edge.Cost;
                        otherSearchNode.NearestNodeToStart = node;
                        if (!priorityQueue.Contains(otherNode))
                            priorityQueue.Add(otherNode);
                    }
                }
                searchNode.WasVisited = true;

                if (node.Coordinates == endPosition)
                    return;

            } while (priorityQueue.Count > 0);
        }

        public List<Node> GetPath() {
            return GetPath(out _);
        }

        public List<Node> GetPath(out float shortestPathCost) {
            Node node = map.Nodes[endPosition.X, endPosition.Y];
            SearchNode searchNode = searchMap[node.Coordinates.X, node.Coordinates.Y];
            List<Node> path = new List<Node>();

            shortestPathCost = 0;
            if (node == null || searchNode.NearestNodeToStart == null) { return path; }

            do {
                searchNode = searchMap[node.Coordinates.X, node.Coordinates.Y];

                path.Add(node);
                if (searchNode.NearestNodeToStart != null) {
                    shortestPathCost += node.Edges.Single(edge =>
                        edge.Nodes.Item1 == searchNode.NearestNodeToStart ||
                        edge.Nodes.Item2 == searchNode.NearestNodeToStart
                    ).Cost;
                }
                node = searchNode.NearestNodeToStart;

                if (node == null || node.Coordinates == startPosition) { break; }
            } while (true);

            path.Reverse();
            return path;
        }

        private void LogHeuristicMap(float[,] heuristicMap) {
            string s = "";
            for (int i = 0; i < heuristicMap.GetLength(1); i++) {
                for (int j = 0; j < heuristicMap.GetLength(0); j++) {
                    s += heuristicMap[j, i].ToString() + "\t";
                }
                s += "\n";
            }
            Debug.Log($"Heuristic map:\n {s}");
        }

        private float[,] SetupHeuristics() {
            float[,] heuristicMap = new float[map.Nodes.GetLength(0), map.Nodes.GetLength(1)];
            for (int i = 0; i < map.Nodes.GetLength(0); i++) {
                for (int j = 0; j < map.Nodes.GetLength(1); j++) {
                    heuristicMap[i, j] = heuristic.Calculate(new Point2DInt(i, j), endPosition);
                }
            }
            return heuristicMap;
        }
    }
}
