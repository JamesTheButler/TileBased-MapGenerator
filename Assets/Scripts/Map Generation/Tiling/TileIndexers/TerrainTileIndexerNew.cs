using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainTileIndexerNew : BaseTileIndexer {
    // Sprites
    public Sprite DefaultSprite;
    public Sprite CenterSprite;
    public Sprite OuterCornerTopRightSprite;
    public Sprite OuterCornerTopLeftSprite;
    public Sprite OuterCornerBottomRightSprite;
    public Sprite OuterCornerBottomLeftSprite;
    public Sprite EdgeTopSprite;
    public Sprite EdgeLeftSprite;
    public Sprite EdgeRightSprite;
    public Sprite EdgeBottomSprite;
    public Sprite InnerCornerTopRightSprite;
    public Sprite InnerCornerTopLeftSprite;
    public Sprite InnerCornerBottomRightSprite;
    public Sprite InnerCornerBottomLeftSprite;

    private Dictionary<int, Tile> indexToTileMap;
    private Tile defaultTile;

    private Tile SpriteToTile(Sprite sprite) {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        return tile;
    }

    void Awake() {
        defaultTile = SpriteToTile(DefaultSprite);
        indexToTileMap = new Dictionary<int, Tile>();
        indexToTileMap.Add(0, SpriteToTile(OuterCornerTopLeftSprite));
        indexToTileMap.Add(1, SpriteToTile(EdgeTopSprite));
        indexToTileMap.Add(2, SpriteToTile(OuterCornerTopRightSprite));
        indexToTileMap.Add(3, SpriteToTile(EdgeLeftSprite));
        indexToTileMap.Add(4, SpriteToTile(CenterSprite));
        indexToTileMap.Add(5, SpriteToTile(EdgeRightSprite));
        indexToTileMap.Add(6, SpriteToTile(OuterCornerBottomLeftSprite));
        indexToTileMap.Add(7, SpriteToTile(EdgeBottomSprite));
        indexToTileMap.Add(8, SpriteToTile(OuterCornerBottomRightSprite));
        indexToTileMap.Add(9, SpriteToTile(InnerCornerTopLeftSprite));
        indexToTileMap.Add(10, SpriteToTile(InnerCornerTopRightSprite));
        indexToTileMap.Add(11, SpriteToTile(InnerCornerBottomLeftSprite));
        indexToTileMap.Add(12, SpriteToTile(InnerCornerBottomRightSprite));
    }

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

        Sprite tileSprite;
        // check edges
        switch (edgeId) {
            case 12: tileSprite = OuterCornerTopLeftSprite; break;
            case 14: tileSprite = EdgeTopSprite; break;
            case 10: tileSprite = OuterCornerTopRightSprite; break;
            case 13: tileSprite = EdgeLeftSprite; break;
            case 15: tileSprite = ResolveCenterPiece(vicinityFlag, cornerId); break;   // center piece
            case 11: tileSprite = EdgeRightSprite; break;
            case 5: tileSprite = OuterCornerBottomLeftSprite; break;
            case 7: tileSprite = EdgeBottomSprite; break;
            case 3: tileSprite = OuterCornerBottomRightSprite; break;
            default: tileSprite = DefaultSprite; break;
        }
        return SpriteToTile(tileSprite);
    }

    private Sprite ResolveCenterPiece(int vicinityFlag, int cornerId) {
        if (cornerId == 0)
            return InnerCornerTopLeftSprite;
        if (cornerId == 1)
            return InnerCornerTopRightSprite;
        if (cornerId == 2)
            return InnerCornerBottomLeftSprite;
        if (cornerId == 3)
            return InnerCornerBottomRightSprite;
        // for all other cases
        return CenterSprite;
    }
}
