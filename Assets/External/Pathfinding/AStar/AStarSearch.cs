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
        private readonly Heuristic heuristic;

        public List<Node> GetPath() { return GetPath(out _); }

        public List<Node> GetPath(out float shortestPathCost) {
            Point2D endPoint = EndPosition;
            Node node = Map.Nodes[endPoint.X, endPoint.Y];
            Debug.Log($"GetPath from {StartPosition} to {EndPosition}");

            List<Node> path = new List<Node>();

            shortestPathCost = 0;

            if (node == null || node.NearestNodeToStart == null) { return path; }

            do {
                if (node == null) { break; }
                path.Add(node);
                if (node.NearestNodeToStart != null) {
                    try {
                        shortestPathCost += node.Edges.Single(edge => edge.Nodes.Item1 == node.NearestNodeToStart || edge.Nodes.Item2 == node.NearestNodeToStart).Cost;
                    } catch (Exception e) {
                        Debug.LogError($"{e.Message} ... nearest node: {node.NearestNodeToStart}");
                    }
                }
                node = node.NearestNodeToStart;
                if (node == null || node.Coordinates == StartPosition) { break; }
            } while (true);

            path.Reverse();
            return path;
        }

        private void Search() {
            float[,] heuristicMap = SetupHeuristics();

            //LogHeuristicMap(heuristicMap);

            Node startNode;
            try {
                startNode = Map.Nodes[StartPosition.X, StartPosition.Y];
            } catch (Exception) {
                throw new Exception("start node must be inside the map");
            }

            startNode.MinCostToStart = 0;
            var priorityQueue = new List<Node>();
            var visitedNodes = new List<Node>();
            priorityQueue.Add(startNode);

            do {
                priorityQueue = priorityQueue.OrderBy(priorityQNode => priorityQNode.MinCostToStart + heuristicMap[priorityQNode.Coordinates.X, priorityQNode.Coordinates.Y]).ToList();
                var node = priorityQueue.First();
                visitedNodes.Add(node);
                priorityQueue.Remove(node);

                foreach (var edge in node.Edges) {
                    Node otherNode;
                    if (edge.Nodes.Item1.Equals(node)) {
                        otherNode = edge.Nodes.Item2;
                    } else {
                        otherNode = edge.Nodes.Item1;
                    }

                    if (otherNode.WasVisited)
                        continue;

                    if (otherNode.MinCostToStart > node.MinCostToStart + edge.Cost) {
                        otherNode.MinCostToStart = node.MinCostToStart + edge.Cost;
                        otherNode.NearestNodeToStart = node;
                        if (!priorityQueue.Contains(otherNode))
                            priorityQueue.Add(otherNode);
                    }
                }
                node.WasVisited = true;
                if (node.Coordinates == EndPosition)
                    return;

            } while (priorityQueue.Count > 0);
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
