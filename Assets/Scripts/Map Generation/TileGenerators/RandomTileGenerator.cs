using UnityEngine;

public class RandomTileGenerator : BaseTileGenerator {
    [Range(0, 1)]
    public float propability;

    /// <summary>
    /// Generates tiles according to the specified heightmap and the cut of value;
    /// </summary>
    public override bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        var thisLayer = base.GenerateTiles(tileTypeMap);

        if (!IsEnabled) return thisLayer;
        if (propability == 0.0f) return thisLayer;

        thisLayer = new bool[tileTypeMap.size.x, tileTypeMap.size.y];
        for (int x = 0; x < tileTypeMap.size.x; x++) {
            for (int y = 0; y < tileTypeMap.size.y; y++) {
                if (UnityEngine.Random.Range(0.0f, 1.0f) < propability) {
                    if (!tileTypeMap.HasAnyTileOnLayers(x, y, blockingTileTypes)) {
                        thisLayer[x, y] = true;
                    }
                }
            }
        }
        return thisLayer;
    }
}
