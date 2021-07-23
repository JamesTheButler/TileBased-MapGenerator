using System.Collections.Generic;

public class CommodityDict : Dictionary<Commodity, double> {
    public CommodityDict() { }
    public CommodityDict(Commodity commodity, double count) {
        Add(commodity, count);
    }
    public CommodityDict(CommodityDict toClone) {
        foreach (var element in toClone) {
            Add(element.Key, element.Value);
        }
    }
}
