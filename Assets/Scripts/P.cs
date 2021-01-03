using System;

/// <summary>
/// Profiler implementation.
/// </summary>
public static class P {
    private static bool isOn;

    public static void SetState(bool is_on) {
        isOn = is_on;
    }

    public static void Start(string name) {
        if(isOn)
            UnityEngine.Profiling.Profiler.BeginSample(name);
    }

    public static void End() {
        if (isOn)
            UnityEngine.Profiling.Profiler.EndSample();
    }

}

