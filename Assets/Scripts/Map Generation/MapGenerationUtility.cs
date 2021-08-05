using UnityEngine;

namespace Assets.Scripts.Map_Generation {
    class MapGenerationUtility : MonoBehaviour {
        private void RenderNoise(float[,] noise, Vector2Int tileMapSize, GameObject debugNoiseQuad) {
            if (debugNoiseQuad == null) return;

            Texture2D texture = new Texture2D(tileMapSize.x, tileMapSize.y, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;
            for (int x = 0; x < tileMapSize.x; x++) {
                for (int y = 0; y < tileMapSize.y; y++) {
                    var noisePixel = noise[x, y];
                    texture.SetPixel(x, y, new Color(noisePixel, noisePixel, noisePixel, 1f));
                }
            }
            debugNoiseQuad.transform.position = new Vector3(tileMapSize.x / 2, tileMapSize.y / 2, -5);
            debugNoiseQuad.transform.localScale = new Vector3(tileMapSize.x, tileMapSize.y, 1);
            texture.Apply();
            debugNoiseQuad.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }
}
