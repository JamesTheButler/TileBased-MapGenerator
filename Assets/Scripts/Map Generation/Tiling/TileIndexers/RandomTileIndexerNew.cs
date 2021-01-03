using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomTileIndexerNew : BaseTileIndexer {
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

    public override Tile Index(int vicinityFlag) {
        var tileId = UnityEngine.Random.Range(0, tileCount);
        return tiles[tileId];
    }
}
