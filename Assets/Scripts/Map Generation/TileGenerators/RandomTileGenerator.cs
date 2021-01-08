using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomTileGenerator : BaseTileGenerator {
    [Range(0, 1)]
    public float propability;

    /// <summary>
    /// Generates tiles according to the specified heightmap and the cut of value;
    /// </summary>
    public override void GenerateTiles(Tilemap tilemap) {
        if (!IsEnabled || tileIndexer == null)
            return;
        if (propability == 0.0f)
            return;
        base.GenerateTiles(tilemap);

        SetTiles(tilemap);
        tileIndexer.Index(tilemap, flagMap, layerHeight);
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
                    if (!tilemap.HasAnyTileOnLayers(x, y, ignoredLayers)) {
                        flagMap[x, y] = true;
                    }
                }
            }
        }
    }
}
