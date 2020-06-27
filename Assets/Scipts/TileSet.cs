using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TileSet {
    public List<Sprite> sprites;
    private List<Tile> tiles;
    [Range (0.0f, 1.0f)]
    public float heightLevel = 0.6f;

    private TileIndexer indexer;

    public void init(TileIndexer indexer) {
        this.indexer = indexer;

        tiles = new List<Tile>();
        foreach (Sprite s in sprites) {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = s;
            t.name = s.name;
            tiles.Add(t);
        }
    }

    public Tile getTile(int id) {    return tiles[Mathf.Clamp(id,0,tiles.Count-1)];   }


    public int getTileIndex(int vicinityFlag){
        return indexer.index(vicinityFlag);
    }

    public Tile getIndexedTile(int vicinityFlag) {
        return tiles[indexer.index(vicinityFlag)];
    }
}
