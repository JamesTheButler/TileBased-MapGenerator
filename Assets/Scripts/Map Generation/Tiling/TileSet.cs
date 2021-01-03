using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Custom implementation of tile set that works with the automatic tile indexers.
/// </summary>
[Serializable]
public class TileSet {
    public Texture2D texture;
    private List<Tile> tiles;
    [Range(0.0f, 1.0f)]
    public float heightLevel = 0.6f;

    private TileIndexer indexer;

    /// <summary>
    /// Initializes the tile set with an indexer.
    /// </summary>
    public void Init(TileIndexer indexer) {
        this.indexer = indexer;

        string spriteSheet = AssetDatabase.GetAssetPath(texture);
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();

        indexer.TileCount = sprites.Length;

        tiles = new List<Tile>();
        foreach (Sprite s in sprites) {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = s;
            t.name = s.name;
            tiles.Add(t);
        }
    }
    /// <summary>
    /// Gets the tile at
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Tile GetTile(int id) {
        return tiles[Mathf.Clamp(id, 0, tiles.Count - 1)];
    }

    /// <summary>
    /// TODO DOC
    /// </summary>
    public int GetTileIndex(int vicinityFlag) {
        return indexer.Index(vicinityFlag);
    }

    /// <summary>
    /// TODO DOC
    /// </summary>
    public Tile GetIndexedTile(int vicinityFlag) {
        int index = indexer.Index(vicinityFlag);
        return tiles[index];
    }
}