/// <summary>
/// Base class for automatic indexing of tiles (i.e. finding the correct sprite index)
/// </summary>
public abstract class TileIndexer {
    public int TileCount { get; set; }

    public abstract int Index(int vicinityFlag);
}
