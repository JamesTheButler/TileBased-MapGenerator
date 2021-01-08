using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Base class for automatic indexing of tiles (i.e. finding the correct sprite index)
/// </summary>
public abstract class BaseTileIndexer : MonoBehaviour{
    public abstract void Index(Tilemap tilemap, bool[,] flagMap, int tileLayer);
}
