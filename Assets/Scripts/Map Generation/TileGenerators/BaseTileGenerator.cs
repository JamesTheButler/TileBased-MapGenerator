using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BaseTileGenerator : MonoBehaviour {
    public Vector2Int tileMapSize;
    public int seed;
    public bool IsEnabled;

    public int layerHeight;
    //public List<TileType> blockTileTypes;

    public BaseTileIndexer tileIndexerPrefab;
    protected BaseTileIndexer tileIndexer;

    protected bool[,] flagMap;

    void Awake() {
        tileIndexer = Instantiate(tileIndexerPrefab);
    }

    /// <summary>
    /// Add tiles to the provided tile map;
    /// </summary>
    public virtual void GenerateTiles(Tilemap tilemap) { }
}
