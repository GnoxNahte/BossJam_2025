using System;
using UnityEngine;

public class DebugTimeScale : MonoBehaviour
{
    [Range(0.01f, 2f)]
    public float TimeScale = 1f;

#if UNITY_EDITOR
    private void Start()
    {
        OnTimeScaleChanged();
    }
#endif

    [VInspector.OnValueChanged("TimeScale")]
    public void OnTimeScaleChanged()
    {
        Time.timeScale = TimeScale;
    }
}
