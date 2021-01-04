using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions {
    public static bool ContainsAnyTileOnLayers(this Tilemap tilemap, int x, int y, List<int> layers) {
        if (layers == null || layers.Count == 0)
            return false;

        foreach (int layer in layers) {
            if (tilemap.HasTile(new Vector3Int(x, y, layer)))
                return true;
        }
        return false;
    }
}
