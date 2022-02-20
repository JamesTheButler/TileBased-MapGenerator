using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomTileIndexer : BaseTileIndexer {
    public Texture2D texture;
    private List<Tile> tiles;
    private int tileCount;

    void Awake() {
        string spriteSheet = AssetDatabase.GetAssetPath(texture);
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
        tileCount = sprites.Length;
        tiles = new List<Tile>();

        foreach (var sprite in sprites) {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = sprite;
            tiles.Add(t);
        }
    }

    public override void Index(Tilemap tilemap, bool[,] flagMap, int tileLayer) {
        base.Index(tilemap, flagMap, tileLayer);
        for (int x = 0; x < flagMap.GetLength(0); x++) {
            for (int y = 0; y < flagMap.GetLength(1); y++) {
                if (flagMap[x, y]) {
                    var pos = new Vector3Int(x, y, tileLayer);
                    var tile = tiles[Random.Range(0, tileCount)];
                    tilemap.SetTile(pos, tile);
                }
            }
        }
    }
}
