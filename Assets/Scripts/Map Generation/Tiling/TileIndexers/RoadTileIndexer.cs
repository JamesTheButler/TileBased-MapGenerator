using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadTileIndexer : BaseTileIndexer {
    // Sprites
    public Sprite Center;
    public Sprite Default;

    public Sprite TSectionTop;
    public Sprite TSectionRight;
    public Sprite TSectionBottom;
    public Sprite TSectionLeft;

    public Sprite CornerTopRight;
    public Sprite CornerBottomRight;
    public Sprite CornerTopLeft;
    public Sprite CornerBottomLeft;

    public Sprite DeadEndTop;
    public Sprite DeadEndLeft;
    public Sprite DeadEndRight;
    public Sprite DeadEndBottom;

    public Sprite Horizontal;
    public Sprite Vertical;

    public override void Index(Tilemap tilemap, bool[,] flagMap, int tileLayer) {
        // Generate vicinity map
        int[,] vicinityMap = GetVicinityMap(flagMap);
        for (int x = 0; x < flagMap.GetLength(0); x++) {
            for (int y = 0; y < flagMap.GetLength(1); y++) {
                if (flagMap[x, y]) {
                    tilemap.SetTile(new Vector3Int(x, y, tileLayer), GetTile(vicinityMap[x, y]));
                }
            }
        }
    }

    private int[,] GetVicinityMap(bool[,] flagMap) {
        int[,] vicinityMap = new int[flagMap.GetLength(0), flagMap.GetLength(1)];

        Vector2Int[,] deltas = new Vector2Int[3, 3];
        deltas[0, 1] = Vector2Int.up;
        deltas[1, 0] = Vector2Int.left;
        deltas[1, 1] = Vector2Int.zero;
        deltas[1, 2] = Vector2Int.right;
        deltas[2, 1] = Vector2Int.down;

        for (int x = 0; x < vicinityMap.GetLength(0); x++) {
            for (int y = 0; y < vicinityMap.GetLength(1); y++) {
                if (flagMap[x, y]) {
                    var pos = new Vector2Int(x, y);
                    //collect surrounding tiles
                    vicinityMap[x, y] = 0;
                    for (int i = 0; i < 3; i++) {
                        for (int j = 0; j < 3; j++) {
                            Vector2Int surroundingPos = pos + deltas[i, j];
                            // if tile is on map
                            if (surroundingPos.x >= 0 && surroundingPos.y >= 0 &&
                                surroundingPos.x < flagMap.GetLength(0) &&
                                surroundingPos.y < flagMap.GetLength(1) &&
                                flagMap[surroundingPos.x, surroundingPos.y]) {

                                vicinityMap[x, y] += 1 << i * 3 + j;
                            }
                        }
                    }
                }
            }
        }
        return vicinityMap;
    }

    private Tile GetTile(int vicinityFlag) {
        // Flags:
        // ____|__1___|____
        // __2_|_tile_|_4_
        // ____|__8___|____
        int edgeId = 0;
        int cornerId = 0;
        // i.e. if vicinityFlag has a 1 in position "TopLeft", then there is a piece in the top left position
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.Top)) != 0) edgeId += 1;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.Left)) != 0) edgeId += 2;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.Right)) != 0) edgeId += 4;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.Bottom)) != 0) edgeId += 8;

        Sprite tileSprite;
        // check edges
        switch (edgeId) {
            case 1: tileSprite = DeadEndBottom; break;
            case 2: tileSprite = DeadEndRight; break;
            case 4: tileSprite = DeadEndLeft; break;
            case 8: tileSprite = DeadEndTop; break;

            case 12: tileSprite = CornerTopLeft; break;
            case 14: tileSprite = TSectionTop; break;
            case 10: tileSprite = CornerTopRight; break;
            case 13: tileSprite = TSectionLeft; break;
            case 15: tileSprite = Center; break;
            case 11: tileSprite = TSectionRight; break;
            case 5: tileSprite = CornerBottomLeft; break;
            case 7: tileSprite = TSectionBottom; break;
            case 3: tileSprite = CornerBottomRight; break;
            case 9: tileSprite = Vertical; break;
            case 6: tileSprite = Horizontal; break;

            default: tileSprite = Default; break;
        }
        return TileUtility.SpriteToTile(tileSprite);
    }
}
