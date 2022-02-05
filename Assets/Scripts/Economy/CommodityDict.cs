using System.Collections.Generic;

public class CommodityDict : Dictionary<Commodity, float> {
    public CommodityDict() { }
    public CommodityDict(Commodity commodity, float count) {
        Add(commodity, count);
    }
    public CommodityDict(CommodityDict toClone) {
        foreach (var element in toClone) {
            Add(element.Key, element.Value);
        }
    }
}
