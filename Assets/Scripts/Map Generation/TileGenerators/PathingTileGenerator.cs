using System.Collections.Generic;
using UnityEngine;

public class PathingTileGenerator : BaseTileGenerator {
    public TileType nodeType;
    public float neighborRadius;
    public TownManager townMgr;

    public override bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        var layer = new bool[tileTypeMap.size.x, tileTypeMap.size.y];
        if (!enabled) return layer;
        if (!tileTypeMap.HasLayer(nodeType)) return layer;

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
        }
        townMgr.SetNeighbors(neighborDict);
        return layer;
    }
}
