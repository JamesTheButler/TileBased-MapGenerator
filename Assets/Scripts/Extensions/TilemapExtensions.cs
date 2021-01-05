using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions {
    public static bool HasAnyTileOnLayers(this Tilemap tilemap, int x, int y, List<TileLayer> layers) {
        if (layers == null || layers.Count == 0)
            return false;

        foreach (var layer in layers) {
            if (tilemap.HasTile(new Vector3Int(x, y, (int)layer)))
                return true;
        }
        return false;
    }
}
