using UnityEngine;
using UnityEngine.Tilemaps;

public class NoiseBasedTileGenerator : BaseTileGenerator {
    public float noiseScale;
    [Range(0.0f, 1.0f)]
    public float heightLevelMin;
    [Range(0.0f, 1.0f)]
    public float heightLevelMax;

    public GameObject noiseQuad;

    protected float[,] noise;

    public override void GenerateTiles(Tilemap tilemap) {
        if (!IsEnabled || tileIndexer == null)
            return;
        base.GenerateTiles(tilemap);

        noise = GeneratePerlinNoise(seed, noiseScale);
        SetTiles(tilemap, noise);
        tileIndexer.Index(tilemap, flagMap, layerHeight);
        RenderNoise(noise);
    }

    /// <summary>
    /// Generates and returns a perlin noise according to the tile map size, the provided seed and the provided noise scale.
    /// </summary>
    private float[,] GeneratePerlinNoise(int seed, float noiseScale) {
        var noise = new float[tileMapSize.x, tileMapSize.y];
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                noise[x, y] = Mathf.PerlinNoise(x / noiseScale + seed, y / noiseScale + seed);
            }
        }
        return noise;
    }

    private void SetTiles(Tilemap tilemap, float[,] noise) {
        var flagCount = 0;
        flagMap = new bool[tileMapSize.x, tileMapSize.y];
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                if (noise[x, y] >= heightLevelMin && noise[x, y] <= heightLevelMax && !tilemap.HasAnyTileOnLayers(x, y, ignoredLayers)) {
                    flagMap[x, y] = true;
                    flagCount++;
                }
            }
        }
    }
    
    /// <summary>
    /// Render noise to the quad (if quad != null).
    /// </summary>
    /// <param name="noise"></param>
    private void RenderNoise(float[,] noise) {
        if (noiseQuad == null)
            return;

        Texture2D texture = new Texture2D(tileMapSize.x, tileMapSize.y, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                var noisePixel = noise[x, y];
                texture.SetPixel(x, y, new Color(noisePixel, noisePixel, noisePixel, 1f));
            }
        }
        noiseQuad.transform.position = new Vector3(tileMapSize.x / 2, tileMapSize.y / 2, -5);
        noiseQuad.transform.localScale = new Vector3(tileMapSize.x, tileMapSize.y, 1);
        texture.Apply();
        noiseQuad.GetComponent<Renderer>().material.mainTexture = texture;
    }
}
