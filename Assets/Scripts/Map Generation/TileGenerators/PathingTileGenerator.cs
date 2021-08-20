using Convenience.Collections.Lists;
using Convenience.Extensions;
using Convenience.Geometry;
using Pathfinding.AStar;
using Pathfinding.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathingTileGenerator : BaseTileGenerator {
    public TileType nodeType;
    public float neighborRadius;

    public event MapGenerator.NeighborsGeneratedEvent OnNeighborsGenerated;

    //TODO: get from somewhere?
    public float PATHCOST = 0.5f;

    public override bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        var thisLayer = base.GenerateTiles(tileTypeMap);
        if (!IsEnabled) return thisLayer;
        if (!tileTypeMap.HasLayer(nodeType)) return thisLayer;

        var nodeTiles = tileTypeMap.GetTiles(nodeType);

        var neighborDict = new Dictionary<Vector2Int, List<Vector2Int>>();

        foreach (var node in nodeTiles) {
            var neighbors = new List<Vector2Int>();

            foreach (var potentialNeighbor in nodeTiles) {
                if (node != potentialNeighbor && Vector2Int.Distance(node, potentialNeighbor) < neighborRadius) {
                    neighbors.Add(potentialNeighbor);
                }

            }
            if (neighbors.IsEmpty().Not()) {
                neighborDict.Add(node, neighbors);
            }
        }
        OnNeighborsGenerated?.Invoke(neighborDict);

        var gridMap = new GridTree2D(tileTypeMap.ToCostField());
        var pathNodes = new List<Vector2Int>();

        var paths = ListDictToTupleList(neighborDict);
        Debug.Log(paths.Aggregate("Paths BEFORE: ", (arg1, arg2) => arg1 + $"<{arg2.Item1}|{arg2.Item2}>"));
        paths.Shuffle(seed);
        Debug.Log(paths.Aggregate("Paths AFTER: ", (arg1, arg2) => arg1 + $"<{arg2.Item1}|{arg2.Item2}>"));
        int i = 0;
        foreach (var entry in paths) {
            i++;
            var start = entry.Item1.ToPoint2D();
            var end = entry.Item2.ToPoint2D();

            var astar = new AStarSearch(gridMap, start, end);
            var path = astar.GetPath().ConvertAll(node => new Vector2Int(node.Coordinates.X, node.Coordinates.Y));

            var intField = gridMap.GetCostField();

            foreach (var node in path) {
                intField[node.x, node.y] = PATHCOST;
            }
            gridMap.Update(intField);
            pathNodes.AddRange(path);
        }

        foreach (var node in pathNodes) {
            thisLayer[node.x, node.y] = true;
        }

        return thisLayer;
    }

    private List<Tuple<Vector2Int, Vector2Int>> ListDictToTupleList(Dictionary<Vector2Int, List<Vector2Int>> dict) {
        var list = new List<Tuple<Vector2Int, Vector2Int>>();
        Debug.Log($"dict has {dict.Keys.Count} keys");

        foreach (var element in dict) {
            foreach (var value in element.Value) {
                var newTuple = new Tuple<Vector2Int, Vector2Int>(element.Key, value);
                if (ContainsTupleOrInverse(list, newTuple).Not()) {
                    list.Add(newTuple);
                }
            }
        }

        return list;
    }

    private bool ContainsTupleOrInverse<T>(List<Tuple<T, T>> tuples, Tuple<T, T> tuple) {
        return tuples.Contains(tuple) || tuples.Contains(new Tuple<T, T>(tuple.Item2, tuple.Item1));
    }
}
