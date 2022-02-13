using Convenience.Collections.Arrays;
using Convenience.Collections.Lists;
using Convenience.Extensions;
using Pathfinding.AStar;
using Pathfinding.General;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathingTileGenerator : BaseTileGenerator {
    public int maxPathCount;
    public TileType nodeType;
    public float neighborRadius;
    public bool enabled2x2Culling = false;
    [SerializeField] private SerializableDict_TileType_float pathingCosts;

    public event MapGenerator.NeighborsGeneratedEvent OnNeighborsGenerated;

    public HeuristicType heuristicType;

    public override bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        var thisLayer = base.GenerateTiles(tileTypeMap);
        if (!IsEnabled) return thisLayer;
        if (!tileTypeMap.HasLayer(nodeType)) return thisLayer;

        // set up paths between nodes
        var nodeTiles = tileTypeMap.GetTiles(nodeType);
        var neighborDict = CreateNeighbors(nodeTiles);
        var nodeConnections = ListDictToTupleList(neighborDict);

        // order list by 
        nodeConnections = nodeConnections.OrderBy(connection => Vector2Int.Distance(connection.Item1, connection.Item2)).ToList();
        LogNodeConnections(nodeConnections);

        //nodeConnections.Shuffle(seed);

        // search heuristic
        Heuristic heuristic;
        if (heuristicType == HeuristicType.MANHATTAN) {
            heuristic = new ManhattanHeuristic();
        } else {
            heuristic = new StraightLineHeuristic();
        }

        // generate path tiles via AStarSearch
        var pathTiles = GeneratePathTiles(tileTypeMap, nodeConnections, heuristic);

        // update layer
        foreach (var pathTile in pathTiles) {
            thisLayer[pathTile.x, pathTile.y] = true;
        }

        // cull 2x2 path blocks
        if (enabled2x2Culling) {
            Cull2x2Crossings(thisLayer, nodeTiles, tileTypeMap.size);
        }
        return thisLayer;
    }

    private void LogNodeConnections(List<Tuple<Vector2Int, Vector2Int>> nodeConnections) {
        var distanceList = nodeConnections.Select(connection => Vector2Int.Distance(connection.Item1, connection.Item2)).ToList();
        var s = "";
        for (var i = 0; i < nodeConnections.Count; i++) {
            s += $"{nodeConnections[i]} - {distanceList[i]}\n";
        }
        Debug.Log(s);
    }

    public float[,] GetTileTypeCostsMap(TileTypeMap tileTypeMap) {
        // Create and fill with GRASS cost
        var costField = Array2DUtility.CreateArray(tileTypeMap.size.x, tileTypeMap.size.y, pathingCosts[TileType.GRASS]);

        foreach (var type in pathingCosts.Keys) {
            // skip GRASS
            if (type == TileType.GRASS) { continue; }

            var layer = tileTypeMap.GetLayer(type);
            costField.MaskedFill(pathingCosts[type], layer);
        }

        return costField;
    }

    private List<Vector2Int> GeneratePathTiles(TileTypeMap tileTypeMap, List<Tuple<Vector2Int, Vector2Int>> paths, Heuristic heuristic) {
        var pathTiles = new List<Vector2Int>();
        var gridMap = new GridTree2D(GetTileTypeCostsMap(tileTypeMap));
        int i = 0;

        foreach (var entry in paths) {
            i++;
            var start = entry.Item1.ToPoint2D();
            var end = entry.Item2.ToPoint2D();
            Debug.Log($"path from {start} to {end}");
            var astar = new AStarSearch(gridMap, start, end, heuristic);
            var path = astar.GetPath().ConvertAll(node => new Vector2Int(node.Coordinates.X, node.Coordinates.Y));

            var intField = gridMap.GetCostField();

            foreach (var node in path) {
                intField[node.x, node.y] = pathingCosts[TileType.ROAD];
            }
            gridMap.Update(intField);
            pathTiles.AddRange(path);

            Debug.Log($"PathingTileGenerator.GeneratePathTiles -- Added path {i}:\n {path.AsString()}.");
            if (i == maxPathCount) break;
        }
        Debug.Log($"PathingTileGenerator.GeneratePathTiles -- Added {i} paths.");

        return pathTiles;
    }

    private Dictionary<Vector2Int, List<Vector2Int>> CreateNeighbors(List<Vector2Int> nodeTiles) {
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
        return neighborDict;
    }

    /* Goes through tile layer and finds all 2x2 patches. */
    private List<Vector2Int> FindTopRightCornersOf2x2Patches(bool[,] pathTileLayer, Vector2Int mapSize) {
        var corners = new List<Vector2Int>();
        // 'mapSize - 1' to not go into corner and get out of bounds with 'pathTileLayer[x + 1, y + 1]'
        for (int x = 0; x < mapSize.x - 1; x++) {
            for (int y = 0; y < mapSize.y - 1; y++) {
                // if all in a 2x2 block are true
                if (pathTileLayer[x, y] && pathTileLayer[x + 1, y] && pathTileLayer[x, y + 1] && pathTileLayer[x + 1, y + 1]) {
                    corners.Add(new Vector2Int(x, y));
                }
            }
        }
        return corners;
    }

    private void Cull2x2Crossings(bool[,] pathTileLayer, List<Vector2Int> nodeTileList, Vector2Int mapSize) {
        var topRightCornersOf2x2Patches = FindTopRightCornersOf2x2Patches(pathTileLayer, mapSize);

        var deltas = new List<Vector2Int> {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
        };

        var cullList = new List<Vector2Int>();

        // foreach 2x2 patch
        foreach (var topTile in topRightCornersOf2x2Patches) {
            var patchTiles = new List<Vector2Int>{
                topTile,
                new Vector2Int(topTile.x + 1, topTile.y),
                new Vector2Int(topTile.x, topTile.y + 1),
                new Vector2Int(topTile.x + 1, topTile.y + 1),
            };

            foreach (var patchTile in patchTiles) {
                var count = 0;
                var hasNode = nodeTileList.Contains(patchTile);
                for (int i = 0; i < 4; i++) {
                    var adjacentTilePos = new Vector2Int(patchTile.x + deltas[i].x, patchTile.y + deltas[i].y);
                    if (adjacentTilePos.IsInside(mapSize)) {
                        if (nodeTileList.Contains(adjacentTilePos)) {
                            hasNode = true;
                            break;
                        }
                        if (pathTileLayer[adjacentTilePos.x, adjacentTilePos.y]) {
                            count++;
                        }
                    }
                }

                if (!hasNode && count == 2) {   // for tiles with 2 neighboring tiles and no adjacent node: add to cull list
                    cullList.Add(patchTile);
                }
            }
        }

        // cull tiles
        foreach (var tile in cullList) {
            pathTileLayer[tile.x, tile.y] = false;
        }
    }

    private List<Tuple<Vector2Int, Vector2Int>> ListDictToTupleList(Dictionary<Vector2Int, List<Vector2Int>> dict) {
        var list = new List<Tuple<Vector2Int, Vector2Int>>();
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
