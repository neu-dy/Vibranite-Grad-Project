using System;
using UnityEngine;

/// Fires ShakeDetected when the controller moves faster than a threshold,
/// but only while the lightstick is on. Speed-only "success" — no timing yet.
public class ShakeDetection : MonoBehaviour
{
    [SerializeField] private Transform trackedController; // controller anchor, child of tracking space
    [SerializeField] private ToggleLightstick lightstick;
    [SerializeField, Tooltip("m/s")] private float speedThreshold = 2.0f;
    [SerializeField, Tooltip("seconds")] private float cooldown = 0.25f;

    public event Action<Vector3> ShakeDetected;

    private Vector3 lastLocalPos;
    private bool hasLastPos;
    private float lastShakeTime = float.NegativeInfinity;

    private void OnEnable() => hasLastPos = false;

    private void Update()
    {
        // Local position = relative to the rig, so future locomotion won't count as a shake.
        Vector3 localPos = trackedController.localPosition;
        float dt = Time.deltaTime;

        if (!hasLastPos || dt <= 0f)
        {
            lastLocalPos = localPos;
            hasLastPos = true;
            return;
        }

        float speed = (localPos - lastLocalPos).magnitude / dt;
        lastLocalPos = localPos;

        if (!lightstick.IsOn) return;
        if (Time.time - lastShakeTime < cooldown) return;
        if (speed < speedThreshold) return;

        lastShakeTime = Time.time;
        ShakeDetected?.Invoke(trackedController.position);
    }
}