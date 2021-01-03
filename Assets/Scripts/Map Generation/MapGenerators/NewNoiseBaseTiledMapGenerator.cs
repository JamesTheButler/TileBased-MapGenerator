using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NewNoiseBaseTiledMapGenerator : MonoBehaviour {
    public Tilemap map;
    public List<BaseTileGenerator> tileGenerators;

    public Vector2Int dimensions;

    public bool generateRandomSeed = false;
    public int seed = 0;

    private void Awake() { }

    private void Start() {
        if (generateRandomSeed)
            GenerateSeed();
        Debug.Log("Map Seed: " + seed);
        foreach (var tileGenerator in tileGenerators) {
            // skip null generators
            if (tileGenerator == null)
                continue;
            tileGenerator.seed = seed;
            tileGenerator.tileMapSize = dimensions;
            tileGenerator.GenerateTiles(map);
        }
    }
    private void GenerateSeed() {
        seed = UnityEngine.Random.Range(0, 10000);
    }
}

