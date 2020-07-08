using System;

public static class P {
    private static bool isOn;

    public static void setState(bool is_on) {
        isOn = is_on;
    }

    public static void start(String name) {
        if(isOn)
            UnityEngine.Profiling.Profiler.BeginSample(name);
    }

    public static void end() {
        if (isOn)
            UnityEngine.Profiling.Profiler.EndSample();
    }

}

