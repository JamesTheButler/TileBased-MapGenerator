using UnityEngine;
using UnityEngine.UI;

public static class UiUtility {
    public static void SetUiText(GameObject gameObject, string text) {
        var label = gameObject.GetComponent<Text>();
        if (label != null) {
            label.text = text;
        } else {
            Debug.LogError($"UIUtility.setUiText -- could not find UI.Text component on game object {gameObject}");
        }
    }
}
