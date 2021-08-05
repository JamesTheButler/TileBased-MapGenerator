using System.Collections.Generic;
using UnityEngine;

public class TileTypeMap {
    public Vector2Int size;

    public TileTypeMap(Vector2Int size) {
        this.size = size;
    }

    private Dictionary<TileType, bool[,]> map = new Dictionary<TileType, bool[,]>();

    public bool HasLayer(TileType tileType) {
        return map.ContainsKey(tileType);
    }

    public bool[,] GetLayer(TileType tileType) {
        return (map.ContainsKey(tileType)) ? map[tileType] : new bool[size.x, size.y];
    }

    public List<Vector2Int> GetTiles(TileType tileType) {
        var tiles = new List<Vector2Int>();
        if (HasLayer(tileType)) {
            var layer = GetLayer(tileType);
            for (int i = 0; i < size.x; i++) {
                for (int j = 0; j < size.y; j++) {
                    if (layer[i, j]) {
                        tiles.Add(new Vector2Int(i, j));
                    }
                }
            }
        }
        return tiles;
    }

    public void SetLayer(TileType tileType, bool[,] layer) {
        if (map.ContainsKey(tileType)) {
            map.Remove(tileType);
        }
        map.Add(tileType, layer);
    }

    public bool HasAnyTileOnLayers(int x, int y, List<TileType> blockingTileTypes) {
        if (blockingTileTypes == null || blockingTileTypes.Count == 0)
            return false;

        foreach (var type in blockingTileTypes) {
            if (!HasLayer(type)) continue;

            var layer = GetLayer(type);
            if (layer[x, y])
                return true;
        }
        return false;
    }
}