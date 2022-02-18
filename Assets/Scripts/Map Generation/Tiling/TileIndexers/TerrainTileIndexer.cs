using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainTileIndexer : BaseTileIndexer {
    // Sprites
    public Sprite DefaultSprite;
    public Sprite CenterSprite;

    [Header("Outer Corners")]
    /* The direction refers to the uncolored part of the sprite. */
    public Sprite OuterCornerTopRightSprite;
    public Sprite OuterCornerTopLeftSprite;
    public Sprite OuterCornerBottomRightSprite;
    public Sprite OuterCornerBottomLeftSprite;

    [Header("Edges")]
    /* The direction refers to uncolored part of the sprite. */
    public Sprite EdgeTopSprite;
    public Sprite EdgeLeftSprite;
    public Sprite EdgeRightSprite;
    public Sprite EdgeBottomSprite;

    [Header("Inner Corners")]
    /* The direction refers to the uncolored part of the sprite. */
    public Sprite InnerCornerTopRightSprite;
    public Sprite InnerCornerTopLeftSprite;
    public Sprite InnerCornerBottomRightSprite;
    public Sprite InnerCornerBottomLeftSprite;

    [Header("T-Sections")]
    /* The direction refers to the narrow part of the sprite. */
    public Sprite TSectionTopSprite;
    public Sprite TSectionRightSprite;
    public Sprite TSectionBottomSprite;
    public Sprite TSectionLeftSprite;

    [Header("Single Bits")]
    /*
     The direction refers to where the connected tile is in relation to this tile.
     The TopSprite is the one with the connective tile above it.
    */
    public Sprite SingleBitTopSprite;
    public Sprite SingleBitRightSprite;
    public Sprite SingleBitBottomSprite;
    public Sprite SingleBitLeftSprite;

    public override void Index(Tilemap tilemap, bool[,] flagMap, int tileLayer) {
        base.Index(tilemap, flagMap, tileLayer);
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
        deltas[0, 0] = Vector2Int.up + Vector2Int.left;
        deltas[0, 1] = Vector2Int.up;
        deltas[0, 2] = Vector2Int.up + Vector2Int.right;
        deltas[1, 0] = Vector2Int.left;
        deltas[1, 1] = Vector2Int.zero;
        deltas[1, 2] = Vector2Int.right;
        deltas[2, 0] = Vector2Int.down + Vector2Int.left;
        deltas[2, 1] = Vector2Int.down;
        deltas[2, 2] = Vector2Int.down + Vector2Int.right;

        for (int x = 0; x < vicinityMap.GetLength(0); x++) {
            for (int y = 0; y < vicinityMap.GetLength(1); y++) {
                if (flagMap[x, y]) {
                    var pos = new Vector2Int(x, y);
                    //collect surrounding tiles
                    vicinityMap[x, y] = 0;
                    for (int i = 0; i < 3; i++) {
                        for (int j = 0; j < 3; j++) {
                            Vector2Int surroundingPos = pos + deltas[i, j];
                            // treat ouside of map as same tile
                            if (surroundingPos.x < 0 || surroundingPos.y < 0 || surroundingPos.x >= flagMap.GetLength(0) || surroundingPos.y >= flagMap.GetLength(1)) {
                                vicinityMap[x, y] += 1 << i * 3 + j;
                                // if terrain tile map has a tile and type == this type
                            } else if (flagMap[surroundingPos.x, surroundingPos.y]) {
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
        // _c1_|__e1__|_c2_
        // _e2_|_tile_|_e4_
        // _c4_|__e8__|_c8_
        int edgeId = 0;
        int cornerId = 0;
        // i.e. if vicinityFlag has a 1 in position "TopLeft", then there is a piece in the top left position
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.Top)) != 0) edgeId += 1;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.Left)) != 0) edgeId += 2;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.Right)) != 0) edgeId += 4;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.Bottom)) != 0) edgeId += 8;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.TopLeft)) != 0) cornerId += 1;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.TopRight)) != 0) cornerId += 2;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.BottomLeft)) != 0) cornerId += 4;
        if ((vicinityFlag & (1 << (int)VicinityFlagEntry.BottomRight)) != 0) cornerId += 8;

        Sprite tileSprite = ResolveTileSprite(edgeId, cornerId);
        if (tileSprite == null) {
            tileSprite = DefaultSprite;
        }
        return TileUtility.SpriteToTile(tileSprite);
    }

    private Sprite ResolveTileSprite(int edgeId, int cornerId) {
        // Flags:
        // _c1_|__e1__|_c2_
        // _e2_|_tile_|_e4_
        // _c4_|__e8__|_c8_
        switch (edgeId) {
            // Outer Corners
            case 12: return OuterCornerTopLeftSprite;
            case 10: return OuterCornerTopRightSprite;
            case 5: return OuterCornerBottomLeftSprite;
            case 3: return OuterCornerBottomRightSprite;
            // Edges
            case 14: return EdgeTopSprite;
            case 13: return EdgeLeftSprite;
            case 11: return EdgeRightSprite;
            case 7: return EdgeBottomSprite;
            // Center Piece
            case 15: return ResolveCenterPiece(cornerId);
            // Single Bits
            case 1: return SingleBitTopSprite;
            case 4: return SingleBitRightSprite;
            case 8: return SingleBitBottomSprite;
            case 2: return SingleBitLeftSprite;

            default: return DefaultSprite;
        }
    }

    private Sprite ResolveCenterPiece(int cornerId) {
        // Flags:
        // _c1_|__e1__|_c2_
        // _e2_|_tile_|_e4_
        // _c4_|__e8__|_c8_
        switch (cornerId) {
            // inner corners
            case 7: return InnerCornerBottomRightSprite;
            case 11: return InnerCornerBottomLeftSprite;
            case 13: return InnerCornerTopRightSprite;
            case 14: return InnerCornerTopLeftSprite;
            // T Sections
            case 12: return TSectionTopSprite;
            case 5: return TSectionRightSprite;
            case 3: return TSectionBottomSprite;
            case 10: return TSectionLeftSprite;

            default: return CenterSprite;
        }
    }
}
