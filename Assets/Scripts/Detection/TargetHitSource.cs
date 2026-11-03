using UnityEngine;

namespace Vibranite.Detection
{
    /// <summary>
    /// Goes on the target object (drum placeholder). Reports a hit whenever
    /// a striker (e.g. the lightstick tip) enters its trigger collider.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TargetHitSource : HitSource
    {
        [Tooltip("Layers that count as a striker, e.g. a 'Striker' layer on the lightstick tip.")]
        [SerializeField] private LayerMask strikerLayers;

        [Tooltip("Minimum seconds between hits, so one swing can't count twice.")]
        [SerializeField, Min(0f)] private float cooldownSeconds = 0.1f;

        private double lastHitDspTime = double.NegativeInfinity;

        // ================= Unity lifecycle =================

        /// <summary>Runs when the component is first added: makes the collider a trigger automatically.</summary>
        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsStriker(other)) return;
            if (!IsOffCooldown()) return;

            RegisterHit();
        }

        // ================= Internals =================

        private bool IsStriker(Collider other)
            => (strikerLayers.value & (1 << other.gameObject.layer)) != 0;

        private bool IsOffCooldown()
            => AudioSettings.dspTime - lastHitDspTime >= cooldownSeconds;

        private void RegisterHit()
        {
            lastHitDspTime = AudioSettings.dspTime;
            ReportHit(lastHitDspTime);
        }
    }
}
