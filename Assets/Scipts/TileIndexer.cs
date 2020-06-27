public class TileIndexer {
    public virtual int index(int vicinityFlag) {
        return 0;
    }
}


public class TerrainTileIndexer : TileIndexer {
    public override int index(int vicinityFlag) {
        // Flags:
        // _._|__1___|_._
        // _8_|_tile_|_2_
        // _._|__4___|_._
        int newTileId = 0;
        switch (vicinityFlag) {
            case 3: newTileId = 6; break;
            case 6: newTileId = 0; break;
            case 7: newTileId = 3; break;
            case 9: newTileId = 8; break;
            case 11: newTileId = 7; break;
            case 12: newTileId = 2; break;
            case 13: newTileId = 5; break;
            case 14: newTileId = 1; break;
            case 15: newTileId = 4; break;
            
            default: newTileId = 4; break; //center piece in any weird case
        }
        return newTileId;
    }
}

public class PathingTileIndexer : TileIndexer {
    public override int index(int vicinityFlag) {
        // Flags:
        // _._|__1___|_._
        // _8_|_tile_|_2_
        // _._|__4___|_._
        int tileId = 0;
        switch (vicinityFlag) {
            case 6: tileId = 0; break;
            case 12: tileId = 1; break;
            case 10: tileId = 2; break;
            case 3: tileId = 3; break;
            case 9: tileId = 4; break;
            case 5: tileId = 5; break;

            default: tileId = 6; break; // filled piece for any weird case
        }
        return tileId;
    }
}