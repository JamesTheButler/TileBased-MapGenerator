using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {
    public Tilemap tileMap;
    private List<BaseTileGenerator> tileGenerators;

    [SerializeField] private SerializableDict_TileType_TileIndexer tileIndexers;
    [SerializeField] private SerializableDict_TileType_int tileLayerHeight;

    public Vector2Int dimensions;

    private TileTypeMap tileTypeMap;

    public bool generateRandomSeed = false;
    public int seed = 0;

    private void Awake() {
        tileGenerators = new List<BaseTileGenerator>(GetComponents<BaseTileGenerator>());
        tileTypeMap = new TileTypeMap(dimensions);
    }

    private void Start() {
        if (generateRandomSeed)
            seed = GenerateSeed();
        Debug.Log("Map Seed: " + seed);
        UnityEngine.Random.InitState(seed);
        foreach (var tileGenerator in tileGenerators) {
            if (tileGenerator == null)
                continue;
            tileGenerator.seed = GenerateSeed();
            var layer = tileGenerator.GenerateTiles(tileTypeMap);
            tileTypeMap.SetLayer(tileGenerator.tileType, layer);
        }
        foreach (var element in tileIndexers) {
            if (tileTypeMap.HasLayer(element.Key)) {
                var layer = tileTypeMap.GetLayer(element.Key);
                // create tile indexer and index
                Instantiate(element.Value, transform).Index(tileMap, layer, tileLayerHeight[element.Key]);
            }
        }
    }

    private int GenerateSeed() {
        return UnityEngine.Random.Range(0, 10000);
    }
}
