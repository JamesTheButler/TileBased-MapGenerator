using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseTiledMapGenerator : MonoBehaviour {
    public List<Sprite> sprites;
    protected List<Tile> base_tiles;
    public Tilemap map;

    void Start() {
        initTiles();
        generateMap();
        updateMap();
    }

    protected virtual void initTiles() { }
    public virtual void generateMap() { }
    public virtual void updateMap() { }

    protected bool tileExists(Vector3Int position) {
        return map.GetTile(position) != null;
    }

    protected virtual void updateTile(Vector3Int pos) {
        if (tileExists(pos)) {
            map.SetTile(pos, base_tiles[getNewTileIndex(getSurroundingFlags(pos))]);
        }
    }

    protected virtual int getSurroundingFlags(Vector3Int pos) {
        return 0;
    }

    protected virtual int getNewTileIndex(int surroundingTileFlags) {
        return 0;
    }
}
