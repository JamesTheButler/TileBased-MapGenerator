using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SingleTileIndexer : BaseTileIndexer {
    public Texture2D tileTexture;
    private Tile tile;

    void Awake() {
        string spriteSheet = AssetDatabase.GetAssetPath(tileTexture);
        Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray()[0];
        tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
    }

    public override Tile Index(int vicinityFlag) {
        return tile;
    }
}
