using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NewNoiseBaseTiledMapGenerator : MonoBehaviour {
    public Tilemap map;
    private List<BaseTileGenerator> tileGenerators;

    public Vector2Int dimensions;

    public bool generateRandomSeed = false;
    public int seed = 0;

    private void Awake() {
        tileGenerators = new List<BaseTileGenerator>(GetComponents<BaseTileGenerator>());
    }

    private void Start() {
        if (generateRandomSeed)
            seed = GenerateSeed();
        Debug.Log("Map Seed: " + seed);
        UnityEngine.Random.InitState(seed);
        foreach (var tileGenerator in tileGenerators) {
            // skip null generators
            if (tileGenerator == null)
                continue;
            tileGenerator.seed = GenerateSeed();
            tileGenerator.tileMapSize = dimensions;
            tileGenerator.GenerateTiles(map);
        }
    }

    private int GenerateSeed() {
        return UnityEngine.Random.Range(0, 10000);
    }
}
