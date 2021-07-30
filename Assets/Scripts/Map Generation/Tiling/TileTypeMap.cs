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
        return (map.ContainsKey(tileType)) ? map[tileType] : null;
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