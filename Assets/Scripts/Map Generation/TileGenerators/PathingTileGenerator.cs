using Convenience.Geometry;
using Pathfinding.AStar;
using Pathfinding.General;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PathingTileGenerator : BaseTileGenerator {
    public TileType nodeType;
    public float neighborRadius;

    public event MapGenerator.NeighborsGeneratedEvent OnNeighborsGenerated;

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
            neighborDict.Add(node, neighbors);
            OnNeighborsGenerated?.Invoke(neighborDict);
        }

        var gridMap = new GridTree2D(tileTypeMap.ToCostField());
        var pathNodes = new List<Vector2Int>();

        var paths = new List<Tuple<Point2D, Point2D>>();

        foreach (var entry in neighborDict) {
            var start = entry.Key.ToPoint();
            var ends = entry.Value.ConvertAll(vector => vector.ToPoint());

            //Debug.Log($"start: {start}");
            //Debug.Log("ends:\n" + ends.Aggregate("", (str, end) => $"{str}{end}\n"));

            foreach (var end in ends) {
                var astar = new AStarSearch(gridMap, start, end);
                var path = astar.GetPath().ConvertAll(node => new Vector2Int(node.Coordinates.X, node.Coordinates.Y));
                //Debug.Log("path:\n" + path.Aggregate("", (str, node) => $"{str}{node}\n"));

                var intField = gridMap.GetCostField();

                foreach (var node in path) {
                    intField[node.x, node.y] = PATHCOST;
                }
                gridMap.Update(intField);

                pathNodes.AddRange(path);
            }
        }

        foreach (var node in pathNodes) {
            thisLayer[node.x, node.y] = true;
        }

        //Debug.Log(ArrayUtil.ToString(layer));
        return thisLayer;
    }

    private bool ContainsTupleOrInverse<T>(List<Tuple<T, T>> tuples, Tuple<T, T> tuple) {
        return tuples.Contains(tuple) || tuples.Contains(new Tuple<T, T>(tuple.Item2, tuple.Item1));
    }
}
