using UnityEngine;
using UnityEngine.UIElements;

public class TownInfoStockListItem : MonoBehaviour {
    [SerializeField] private GameObject nameText;
    [SerializeField] private GameObject amountText;
    [SerializeField] private GameObject amountChangeText;
    [SerializeField] private GameObject priceText;

    public void UpdateListItem(string name, float amount, float production, float consumption, float totalChange, float price) {
        UiUtility.SetUiText(nameText, name);
        UiUtility.SetUiText(amountText, FormateFloat(amount));
        UiUtility.SetUiText(amountChangeText, $"{FormateFloat(totalChange)}(+{FormateFloat(production)} / -{FormateFloat(consumption)})");
        UiUtility.SetUiText(priceText, FormateFloat(price));
    }

    private string FormateFloat(float d) { return d.ToString("0.0"); }
}
