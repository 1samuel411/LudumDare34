using UnityEngine;
using System.Collections;

public class CoreTimer
{
    public float maxTime = 5.0f;
    public float breakTime = 0.0f;
    public float currentTime = 0.0f;

    public void ResetTimer() {
        currentTime = maxTime;
    }

    public bool CheckForBreak() {
        return (currentTime <= breakTime);
    }

    public void DecrementTime() {
        currentTime -= Time.deltaTime;
    }
}
