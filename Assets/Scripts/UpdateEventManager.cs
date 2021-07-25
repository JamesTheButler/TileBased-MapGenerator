using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class UpdateEventManager : MonoBehaviour {
    public delegate void UpdateEvent();
    public static event UpdateEvent OnUpdate;
    public static event UpdateEvent OnTownPopulationUpdate;
    [SerializeField] float townPopulationUpdateInterval = 5.0f;
    public static event UpdateEvent OnTownStockUpdate;
    [SerializeField] float townStockUpdateInterval = 1.0f;

    //Dictionary<IntervalType, IntervalData> intervalTable;
    [Serializable] public class ITtoFDict : SerializableDictionaryBase<IntervalType, float> { }
    [SerializeField] ITtoFDict updateEventIntervalTable = new ITtoFDict();
    ITtoFDict updatEventLastUpdateTable = new ITtoFDict();
    public enum IntervalType {
        BASE_UPDATE,
        TOWN_STOCK,
        TOWN_POPULATION,
    }

    private void Invoke(IntervalType type) {
        switch (type) {
            case IntervalType.BASE_UPDATE: OnUpdate?.Invoke(); break;
            case IntervalType.TOWN_POPULATION: OnTownPopulationUpdate?.Invoke(); break;
            case IntervalType.TOWN_STOCK: OnTownStockUpdate?.Invoke(); break;
        }
    }

    private void Update() {
        OnUpdate?.Invoke();
        var now = Time.time;

        foreach (var updateEventType in updateEventIntervalTable.Keys) {
            if (!updatEventLastUpdateTable.ContainsKey(updateEventType)) {
                updatEventLastUpdateTable.Add(updateEventType, 0);
            }

            var lastUpdate = updatEventLastUpdateTable[updateEventType];
            var intervalTime = updateEventIntervalTable[updateEventType];
            if (now - lastUpdate > intervalTime) {
                updatEventLastUpdateTable[updateEventType] = now;
                Invoke(updateEventType);
                Debug.Log($"Updateeventmgr updated {updateEventType.ToString()}");
            }
        }
    }
}
