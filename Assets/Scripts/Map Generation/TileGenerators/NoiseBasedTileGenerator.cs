using System.Collections.Generic;
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

        noise = GeneratePerlinNoise(seed, noiseScale);
        SetTiles(tilemap, noise);
        IndexTiles(tilemap);
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

    private void IndexTiles(Tilemap tilemap) {
        Vector2Int pos = new Vector2Int(0, 0);
        Vector2Int[,] deltas = new Vector2Int[3, 3];
        deltas[0, 0] = Vector2Int.up + Vector2Int.left;
        deltas[0, 1] = Vector2Int.up;
        deltas[0, 2] = Vector2Int.up + Vector2Int.right;
        deltas[1, 0] = Vector2Int.left;
        deltas[1, 1] = Vector2Int.zero;
        deltas[1, 2] = Vector2Int.right;
        deltas[2, 0] = Vector2Int.down + Vector2Int.left;
        deltas[2, 1] = Vector2Int.down;
        deltas[2, 2] = Vector2Int.down + Vector2Int.right;
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                if (flagMap[x, y]) {
                    //collect surrounding tiles
                    int flags = 0;
                    for (int i = 0; i < 3; i++) {
                        for (int j = 0; j < 3; j++) {
                            Vector2Int surroundingPos = pos + deltas[i, j];
                            // treat ouside of map as same tile
                            if (!surroundingPos.IsInside(new Vector2Int(), tileMapSize - Vector2Int.one)) {
                                flags += 1 << i * 3 + j;
                                // if terrain tile map has a tile and type == this type
                            } else if (flagMap[surroundingPos.x, surroundingPos.y]) {
                                flags += 1 << i * 3 + j;
                            }
                        }
                    }
                    tilemap.SetTile(new Vector3Int(x, y, layerHeight), tileIndexer.Index(flags));
                }
            }
        }
    }

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