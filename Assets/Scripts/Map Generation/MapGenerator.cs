using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {
    public Tilemap tileMap;
    private List<BaseTileGenerator> tileGenerators;

    [SerializeField] private SerializableDict_TileType_TileIndexer tileIndexers;
    [SerializeField] private SerializableDict_TileType_int tileLayerHeight;

    public delegate void MapGenerationEvent(TileTypeMap tileTypeMap);
    public static event MapGenerationEvent OnMapGenerationFinished;

    public delegate void NeighborsGeneratedEvent(Dictionary<Vector2Int, List<Vector2Int>> neighbors);
    public static event NeighborsGeneratedEvent OnNeighborsGenerated;

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
            if (tileGenerator == null || !tileGenerator.IsEnabled) { continue; }

            if (tileGenerator is PathingTileGenerator generator && tileGenerator.tileType == TileType.ROAD) {
                generator.OnNeighborsGenerated += InvokeOnNeighborsGenerated;
            }
            tileGenerator.seed = GenerateSeed();
            var layer = tileGenerator.GenerateTiles(tileTypeMap);
            tileTypeMap.SetLayer(tileGenerator.tileType, layer);
        }

        foreach (var tileType in tileIndexers.Keys) {
            if (tileTypeMap.HasLayer(tileType)) {
                var layer = tileTypeMap.GetLayer(tileType);
                // create tile indexer and index tiles
                Instantiate(tileIndexers[tileType], transform).Index(tileMap, layer, tileLayerHeight[tileType]);
            }
        }

        OnMapGenerationFinished?.Invoke(tileTypeMap);
    }

    private void InvokeOnNeighborsGenerated(Dictionary<Vector2Int, List<Vector2Int>> neighbors) {
        OnNeighborsGenerated?.Invoke(neighbors);
    }

    private int GenerateSeed() {
        return UnityEngine.Random.Range(0, 10000);
    }
}
