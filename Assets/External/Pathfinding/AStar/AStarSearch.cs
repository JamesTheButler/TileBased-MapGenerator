using Convenience.Collections.Arrays;
using Convenience.Collections.Lists;
using Convenience.Geometry;
using Pathfinding.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding.AStar {
    public class AStarSearch {
        public AStarSearch(GridTree2D map, Point2D startNode, Point2D endNode, Heuristic heuristic) {
            Map = map;
            StartPosition = startNode;
            EndPosition = endNode;

            this.heuristic = heuristic;
            Search();
        }

        public GridTree2D Map { get; set; }
        public Point2D StartPosition { get; set; }
        public Point2D EndPosition { get; set; }
        private SearchNode[,] SearchMap { get; set; }
        private readonly Heuristic heuristic;

        private void Search() {
            float[,] heuristicMap = SetupHeuristics();
            //LogHeuristicMap(heuristicMap);

            SearchMap = new SearchNode[Map.Width, Map.Height];
            for (int i = 0; i < Map.Width; i++) {
                for (int j = 0; j < Map.Height; j++) {
                    SearchMap[i, j] = new SearchNode();
                }
            }

            Node startNode = Map.Nodes[StartPosition.X, StartPosition.Y];
            var startSearchNode = SearchMap[StartPosition.X, StartPosition.Y];

            startSearchNode.MinCostToStart = 0;
            var priorityQueue = new List<Node>();
            var visitedNodes = new List<Node>();
            priorityQueue.Add(startNode);

            do {
                // sort priority queue by smallest costToStart+heuristic
                priorityQueue = priorityQueue.OrderBy(priorityQNode => {
                    var priorityQSearchNode = SearchMap[priorityQNode.Coordinates.X, priorityQNode.Coordinates.Y];
                    var heuristic = heuristicMap[priorityQNode.Coordinates.X, priorityQNode.Coordinates.Y];
                    return priorityQSearchNode.MinCostToStart + heuristic;
                }).ToList();

                var node = priorityQueue.PopFirst();
                var searchNode = SearchMap[node.Coordinates.X, node.Coordinates.Y];
                visitedNodes.Add(node);

                foreach (var edge in node.Edges) {
                    Node otherNode;
                    SearchNode otherSearchNode;
                    if (edge.Nodes.Item1.Equals(node)) {
                        otherNode = edge.Nodes.Item2;
                    } else {
                        otherNode = edge.Nodes.Item1;
                    }
                    otherSearchNode = SearchMap[otherNode.Coordinates.X, otherNode.Coordinates.Y];

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

                if (node.Coordinates == EndPosition)
                    return;

            } while (priorityQueue.Count > 0);
        }

        public List<Node> GetPath() {
            return GetPath(out _);
        }

        public List<Node> GetPath(out float shortestPathCost) {
            Node node = Map.Nodes[EndPosition.X, EndPosition.Y];
            SearchNode searchNode = SearchMap[node.Coordinates.X, node.Coordinates.Y];
            List<Node> path = new List<Node>();

            shortestPathCost = 0;
            if (node == null || searchNode.NearestNodeToStart == null) { return path; }

            do {
                searchNode = SearchMap[node.Coordinates.X, node.Coordinates.Y];

                path.Add(node);
                if (searchNode.NearestNodeToStart != null) {
                    shortestPathCost += node.Edges.Single(edge =>
                        edge.Nodes.Item1 == searchNode.NearestNodeToStart ||
                        edge.Nodes.Item2 == searchNode.NearestNodeToStart
                    ).Cost;
                }
                node = searchNode.NearestNodeToStart;
                
                if (node == null || node.Coordinates == StartPosition) { break; }
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
            float[,] heuristicMap = new float[Map.Nodes.GetLength(0), Map.Nodes.GetLength(1)];
            for (int i = 0; i < Map.Nodes.GetLength(0); i++) {
                for (int j = 0; j < Map.Nodes.GetLength(1); j++) {
                    heuristicMap[i, j] = heuristic.Calculate(Map.Nodes[i, j].Coordinates, EndPosition);
                }
            }
            return heuristicMap;
        }
    }
}
