public class TestTileGenerator : BaseTileGenerator {
    public override bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        var thisLayer = base.GenerateTiles(tileTypeMap);

        if (!IsEnabled) return thisLayer;

        for (int x = 0; x < tileTypeMap.size.x; x++) {
            for (int y = 0; y < tileTypeMap.size.y; y++) {
                thisLayer[x, y] = true;
            }
        }
        return thisLayer;
    }
}
