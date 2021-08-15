using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MouseInput : MonoBehaviour {
    public GameObject tileHighlight;
    public Tilemap tileMap;
    public Text textField;
    public TownManager townMgr;

    private void Update() {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int roundedMouseWorldPos = new Vector2Int(Mathf.FloorToInt(mouseWorldPos.x), Mathf.FloorToInt(mouseWorldPos.y));

        tileHighlight.transform.position = new Vector3(roundedMouseWorldPos.x, roundedMouseWorldPos.y, -1);

        if (Input.GetMouseButtonDown(0)) {
            var town = townMgr.GetTown(roundedMouseWorldPos);
            if (town != null) {
                Debug.Log($"MouseInput -- clicked on Town {town.Name}");
                textField.text = $"Town: {town.Name} [{town.Coordinates}]";
            } else {
                textField.text = string.Empty;
            }
        }
    }
}
