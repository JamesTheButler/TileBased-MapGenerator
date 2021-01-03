using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomTileGenerator : BaseTileGenerator {
    [Range(0, 1)]
    public float propability;
    public List<int> blockingLayers;

    /// <summary>
    /// Generates tiles according to the specified heightmap and the cut of value;
    /// </summary>
    public override void GenerateTiles(Tilemap tilemap) {
        if (!IsEnabled || tileIndexer == null)
            return;

        if (propability == 0.0f)
            return;

        SetTiles(tilemap);
        IndexTiles(tilemap);
    }

    /// <summary>
    /// Randomly fills the flag map with tiles.
    /// </summary>
    private void SetTiles(Tilemap tilemap) {
        UnityEngine.Random.InitState(seed);
        flagMap = new bool[tileMapSize.x, tileMapSize.y];
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                if (UnityEngine.Random.Range(0.0f, 1.0f) < propability) {
                    if (!IsBlocked(tilemap, blockingLayers, new Vector2Int(x, y))) {
                        flagMap[x, y] = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check if the provided tile map contains a tile on one of the provided layers in the provided position.
    /// </summary>
    private bool IsBlocked(Tilemap tilemap, List<int> blockingLayers, Vector2Int pos) {
        foreach (var blockingLayer in blockingLayers) {
            if (tilemap.HasTile(new Vector3Int(pos.x, pos.y, blockingLayer))) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Indexes all tiles.
    /// </summary>
    private void IndexTiles(Tilemap tilemap) {
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                if (flagMap[x, y]) {
                    tilemap.SetTile(new Vector3Int(x, y, layerHeight), tileIndexer.Index(0));
                }
            }
        }
    }
}
