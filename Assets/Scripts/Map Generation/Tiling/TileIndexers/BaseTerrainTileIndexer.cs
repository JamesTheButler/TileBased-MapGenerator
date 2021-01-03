using UnityEngine.Tilemaps;

/// <summary>
/// Base class for different terrain tile indexers. Allows for a standardized way of process vicinity flags.
/// </summary>
//TODO: Not sure if extending a class for one generic function is good. (Actually, I'm sure it's not.)
public class BaseTerrainTileIndexer : BaseTileIndexer {
    public override Tile Index(int vicinityFlag) {
        return null;
    }

    protected int GetTileIndex(int vicinityFlag) {
        // Flags:
        // _c1_|__e1___|_c2_
        // _e2_|_tile_|_e4_
        // _c4_|__e8___|_c8_
        int edgeId = 0;
        int cordnerId = 0;
        if ((vicinityFlag & (1 << 0)) != 0) cordnerId += 1;
        if ((vicinityFlag & (1 << 1)) != 0) edgeId += 1;
        if ((vicinityFlag & (1 << 2)) != 0) cordnerId += 2;
        if ((vicinityFlag & (1 << 3)) != 0) edgeId += 2;
        if ((vicinityFlag & (1 << 5)) != 0) edgeId += 4;
        if ((vicinityFlag & (1 << 6)) != 0) cordnerId += 4;
        if ((vicinityFlag & (1 << 7)) != 0) edgeId += 8;
        if ((vicinityFlag & (1 << 8)) != 0) cordnerId += 8;

        int tileId = -1;
        switch (edgeId) {
            case 12: tileId = 0; break;
            case 14: tileId = 1; break;
            case 10: tileId = 2; break;
            case 13: tileId = 3; break;
            case 15: tileId = 4; break;
            case 11: tileId = 5; break;
            case 5: tileId = 6; break;
            case 7: tileId = 7; break;
            case 3: tileId = 8; break;
            default: tileId = -1; break;
        }

        //special cases with missing corner
        if (tileId == 4 && cordnerId != 15) {
            if ((vicinityFlag & (1 << 0)) == 0) tileId = 12;
            else if ((vicinityFlag & (1 << 2)) == 0) tileId = 11;
            else if ((vicinityFlag & (1 << 8)) == 0) tileId = 9;
            else if ((vicinityFlag & (1 << 6)) == 0) tileId = 10;
        }
        return tileId;
    }
}
