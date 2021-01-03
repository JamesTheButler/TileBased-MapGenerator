using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DonutTiledMapGenerator : BaseTiledMapGenerator {
    public int outer_diameter;
    public int inner_diameter;
    public bool use_miss_chance = true;
    [Range(0.0f, 1.0f)]
    public float miss_chance = 0.5f;
    public int depth;

    protected override void InitTiles() {
        baseTiles = new List<Tile>();
        foreach (Sprite s in sprites) {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = s;
            t.name = s.name;
            baseTiles.Add(t);
        }
    }

    public override void GenerateMap() {
        Debug.Log("generateCircularMap");
        Vector3Int center = new Vector3Int(outer_diameter / 2, outer_diameter / 2, 0);
        Vector3Int position = new Vector3Int(0, 0, depth);
        for (int x = 0; x < outer_diameter; x++) {
            for (int y = 0; y < outer_diameter; y++) {
                float dist = Vector3Int.Distance(new Vector3Int(x, y, 0), center);
                if (dist-.5f <= outer_diameter/2 && dist >= inner_diameter / 2) {
                    if (use_miss_chance && Random.Range(0.0f, 1.0f) > miss_chance) {
                        position.x = x;
                        position.y = y;
                        map.SetTile(position, baseTiles[4]);
                    }
                }
            }
        }
    }
    
    public override void UpdateMap() {
        Debug.Log("updateMap");
        Vector3Int position = new Vector3Int(0, 0, depth);
        for (int x = 0; x < outer_diameter; x++) {
            for (int y = 0; y < outer_diameter; y++) {
                position.x = x;
                position.y = y;
                UpdateTile(position);
            }
        }
    }

    protected override int GetSurroundingFlags(Vector3Int pos) {
        int surroundingTileFlags = 0;
        surroundingTileFlags += TileExists(pos + new Vector3Int(0, 1, 0)) ? 1 : 0;
        surroundingTileFlags += TileExists(pos + new Vector3Int(1, 0, 0)) ? 2 : 0;
        surroundingTileFlags += TileExists(pos + new Vector3Int(0, -1, 0)) ? 4 : 0;
        surroundingTileFlags += TileExists(pos + new Vector3Int(-1, 0, 0)) ? 8 : 0;
        return surroundingTileFlags;
    }

    protected override int GetNewTileIndex(int surroundingTileFlags) {
        //_._|__1__|_._
        //_8_|_pos_|_2_
        // . |  4  | .

        int newTileId = 0;
        switch (surroundingTileFlags) {
            //case 0: break;
            //case 1: break;
            //case 2: break;
            case 3: newTileId = 6; break;
            //case 4: break;
            case 5: newTileId = 3; break;
            case 6: newTileId = 0; break;
            case 7: newTileId = 3; break;
            //case 8: break;
            case 9: newTileId = 8; break;
            case 10: newTileId = 1; break;
            case 11: newTileId = 7; break;
            case 12: newTileId = 2; break;
            case 13: newTileId = 5; break;
            case 14: newTileId = 1; break;
            case 15: newTileId = 4; break;

            default: newTileId = 4; break;
        }
        return newTileId;
    }
}
