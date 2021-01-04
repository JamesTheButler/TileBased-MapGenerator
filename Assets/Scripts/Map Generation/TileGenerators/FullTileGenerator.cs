using UnityEngine;
using UnityEngine.Tilemaps;

public class FullTileGenerator : BaseTileGenerator {
    public override void GenerateTiles(Tilemap tilemap) {
        if (!IsEnabled)
            return;

        base.GenerateTiles(tilemap);
        for (int x = 0; x < tileMapSize.x; x++) {
            for (int y = 0; y < tileMapSize.y; y++) {
                tilemap.SetTile(new Vector3Int(x, y, layerHeight), tileIndexer.Index(0));
            }
        }
    }
}
