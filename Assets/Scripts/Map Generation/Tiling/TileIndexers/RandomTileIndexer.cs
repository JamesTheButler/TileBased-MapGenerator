using UnityEngine;
/// <summary>
/// Picks a random tile index.
/// </summary>
public class RandomTileIndexer : TileIndexer {
    public override int Index(int vicinityFlag) {
        return Random.Range(0, TileCount);
    }
}