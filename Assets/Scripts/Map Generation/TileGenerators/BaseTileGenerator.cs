using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTileGenerator : MonoBehaviour {
    public bool IsEnabled = true;
    public int seed;
    public TileType tileType;

    public List<TileType> blockingTileTypes;

    public abstract bool[,] GenerateTiles(TileTypeMap tileTypeMap);
}
