using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class SingleTileIndexer : BaseTileIndexer {
    public Texture2D tileTexture;
    private Tile tile;

    void Awake() {
        string spriteSheet = AssetDatabase.GetAssetPath(tileTexture);
        Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray()[0];
        tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
    }

    public override void Index(Tilemap tilemap, bool[,] flagMap, int tileLayer) {
        base.Index(tilemap, flagMap, tileLayer);
        for (int x = 0; x < flagMap.GetLength(0); x++) {
            for (int y = 0; y < flagMap.GetLength(1); y++) {
                if (flagMap[x, y]) {
                    tilemap.SetTile(new Vector3Int(x, y, tileLayer), tile);
                }
            }
        }
    }
}
