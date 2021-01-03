using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Base class for tiled map generators.
/// </summary>
public class BaseTiledMapGenerator : MonoBehaviour {
    public List<Sprite> sprites;
    protected List<Tile> baseTiles;
    public Tilemap map;

    void Start() {
        InitTiles();
        GenerateMap();
        UpdateMap();
    }

    /// <summary>
    /// TODO DOC
    /// </summary>
    protected virtual void InitTiles() { }

    /// <summary>
    /// TODO DOC
    /// </summary>
    public virtual void GenerateMap() { }

    /// <summary>
    /// TODO DOC
    /// </summary>
    public virtual void UpdateMap() { }

    /// <summary>
    /// Determines if a tile is set at the specified position.
    /// </summary>
    protected bool TileExists(Vector3Int position) {
        return map.GetTile(position) != null;
    }

    /// <summary>
    /// Updates a tile based on the surrounding tiles.
    /// </summary>
    /// <param name="pos"></param>
    protected virtual void UpdateTile(Vector3Int pos) {
        if (TileExists(pos)) {
            map.SetTile(pos, baseTiles[GetNewTileIndex(GetSurroundingFlags(pos))]);
        }
    }

    /// <summary>
    /// Gets the vicinity flag at the specified position.
    /// </summary>
    protected virtual int GetSurroundingFlags(Vector3Int pos) {
        return 0;
    }

    /// <summary>
    /// TODO DOC
    /// </summary>
    protected virtual int GetNewTileIndex(int surroundingTileFlags) {
        return 0;
    }
}
