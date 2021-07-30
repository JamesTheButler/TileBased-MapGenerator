using UnityEngine;
using UnityEngine.UIElements;

public class TownInfoStockListItem : MonoBehaviour {
    [SerializeField] private GameObject nameText;
    [SerializeField] private GameObject amountText;
    [SerializeField] private GameObject amountChangeText;
    [SerializeField] private GameObject priceText;

    public void UpdateListItem(string name, double amount, double production, double consumption, double totalChange, double price) {
        UiUtility.SetUiText(nameText, name);
        UiUtility.SetUiText(amountText, FormateDouble(amount));
        UiUtility.SetUiText(amountChangeText, $"{FormateDouble(totalChange)}(+{FormateDouble(production)} / -{FormateDouble(consumption)})");
        UiUtility.SetUiText(priceText, FormateDouble(price));
    }

    private string FormateDouble(double d) { return d.ToString("0.0"); }
}
