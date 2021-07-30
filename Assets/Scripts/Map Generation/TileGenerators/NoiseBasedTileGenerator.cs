using UnityEngine;

public class NoiseBasedTileGenerator : BaseTileGenerator {
    public float noiseScale;
    [Range(0.0f, 1.0f)]
    public float heightLevelMin;
    [Range(0.0f, 1.0f)]
    public float heightLevelMax;

    public GameObject debugNoiseQuad;

    protected float[,] noise;

    public override bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        var thisLayer = new bool[tileTypeMap.size.x, tileTypeMap.size.y];

        if (!IsEnabled) return thisLayer;

        SetTiles(
            tileTypeMap,
            GeneratePerlinNoise(seed, noiseScale, tileTypeMap.size),
            tileTypeMap.size,
            ref thisLayer
            );
        RenderNoise(noise, tileTypeMap.size);
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

    private void RenderNoise(float[,] noise, Vector2Int tileMapSize) {
        if (debugNoiseQuad == null) return;

        Texture2D texture = new Texture2D(tileMapSize.x, tileMapSize.y, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                var noisePixel = noise[x, y];
                texture.SetPixel(x, y, new Color(noisePixel, noisePixel, noisePixel, 1f));
            }
        }
        debugNoiseQuad.transform.position = new Vector3(tileMapSize.x / 2, tileMapSize.y / 2, -5);
        debugNoiseQuad.transform.localScale = new Vector3(tileMapSize.x, tileMapSize.y, 1);
        texture.Apply();
        debugNoiseQuad.GetComponent<Renderer>().material.mainTexture = texture;
    }
}
