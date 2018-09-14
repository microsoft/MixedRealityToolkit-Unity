using System.Linq;
using UnityEngine;

public class FpsUseTracker
{
    private int index = 0;
    private float[] timings = new float[10];

    public float GetFpsInSeconds()
    {
        timings[index] = Time.unscaledDeltaTime;
        index = (index + 1) % timings.Length;

        return timings.Average();
    }
}
