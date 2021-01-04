using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BaseTileGenerator : MonoBehaviour {
    public bool IsEnabled = true;
    public Vector2Int tileMapSize;
    public int seed;

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
