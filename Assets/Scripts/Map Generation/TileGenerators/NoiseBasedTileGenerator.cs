using UnityEngine;

public class NoiseBasedTileGenerator : BaseTileGenerator {
    public float noiseScale;
    [Range(0.0f, 1.0f)]
    public float heightLevelMin;
    [Range(0.0f, 1.0f)]
    public float heightLevelMax;

    protected float[,] noise;

    public override bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        var thisLayer = base.GenerateTiles(tileTypeMap);

        if (!IsEnabled) return thisLayer;

        SetTiles(
            tileTypeMap,
            GeneratePerlinNoise(seed, noiseScale, tileTypeMap.size),
            tileTypeMap.size,
            ref thisLayer
            );
        return thisLayer;
    }

    private float[,] GeneratePerlinNoise(int seed, float noiseScale, Vector2Int tileMapSize) {
        var noise = new float[tileMapSize.x, tileMapSize.y];
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                noise[x, y] = Mathf.PerlinNoise(x / noiseScale + seed, y / noiseScale + seed);
            }
        }
        return noise;
    }

    private void SetTiles(TileTypeMap tileTypeMap, float[,] noise, Vector2Int tileMapSize, ref bool[,] thisLayer) {
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                if (noise[x, y] >= heightLevelMin && noise[x, y] <= heightLevelMax && !tileTypeMap.HasAnyTileOnLayers(x, y, blockingTileTypes)) {
                    thisLayer[x, y] = true;
                }
            }
        }
    }
}
