using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MouseInput : MonoBehaviour {
    public GameObject tileHighlight;
    public Tilemap tileMap;
    public Text textField;
    public TownManager townMgr;

    // find mouse position continously
    // if it is over a tile: place hightlight

    // on click:

    private void Update() {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int roundedMouseWorldPos = new Vector2Int(Mathf.FloorToInt(mouseWorldPos.x), Mathf.FloorToInt(mouseWorldPos.y));

        tileHighlight.transform.position = new Vector3(roundedMouseWorldPos.x + 0.5f, roundedMouseWorldPos.y + 0.5f, -1);

        if (Input.GetMouseButtonDown(0)) {
            var town = townMgr.GetTown(new Vector2Int(roundedMouseWorldPos.x, roundedMouseWorldPos.y));
            if (town != null) {
                textField.text = $"Town: {town.Name}";
            } else {
                textField.text = $"No Town";
            }
        }
    }
}
