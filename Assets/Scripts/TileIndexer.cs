using UnityEngine;

public class TileIndexer {
    public int tileCount;
    public virtual int index(int vicinityFlag) {
       // Debug.Log("Indexer: " + this.GetType().Name);
        return 0;
    }
}

public class TerrainTileIndexer : TileIndexer {
    public override int index(int vicinityFlag) {   
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

        int tileId = 0;
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

            default: tileId = 4; break; //center piece in any weird case
        }
        //special cases with missing corner
        if (tileId == 4 && cordnerId != 15) {
            if ((vicinityFlag & (1 << 0)) == 0) tileId = 12;
            else if ((vicinityFlag & (1 << 2)) == 0) tileId = 11;
            else if ((vicinityFlag & (1 << 8)) == 0) tileId = 9;
            else if ((vicinityFlag & (1 << 6)) == 0) tileId = 10;
        }

      //  Debug.Log("tileId: " + tileId);
        return tileId;
    }
}

public class PathingTileIndexer : TileIndexer {
    public override int index(int vicinityFlag) {
        // Flags:
        // _c1_|__e1___|_c2_
        // _e2_|_tile_|_e4_
        // _c4_|__e8___|_c8_

        int edgeId = 0;
        //int cordnerId = 0;
        //if ((vicinityFlag & (1 << 0)) != 0) cordnerId += 1;
        if ((vicinityFlag & (1 << 1)) != 0) edgeId += 1;
        //if ((vicinityFlag & (1 << 2)) != 0) cordnerId += 2;
        if ((vicinityFlag & (1 << 3)) != 0) edgeId += 2;
        if ((vicinityFlag & (1 << 5)) != 0) edgeId += 4;
        //if ((vicinityFlag & (1 << 6)) != 0) cordnerId += 4;
        if ((vicinityFlag & (1 << 7)) != 0) edgeId += 8;
        //if ((vicinityFlag & (1 << 8)) != 0) cordnerId += 8;

        int tileId = 0;
        switch (edgeId) {
            case 12: tileId = 0; break;
            case 10: tileId = 1; break;
            case 6: tileId = 2; break;
            case 5: tileId = 3; break;
            case 3: tileId = 4; break;
            case 9: tileId = 5; break;
            case 2: tileId = 7; break;
            case 1: tileId = 8; break;
            case 8: tileId = 9; break;
            case 4: tileId = 0; break;

            default: tileId = 6; break; // full piece
        }
        return tileId;
    }
}

public class RandomTileIndexer : TileIndexer {
    public override int index(int vicinityFlag) {
        return Random.Range(0,tileCount);
    }
}