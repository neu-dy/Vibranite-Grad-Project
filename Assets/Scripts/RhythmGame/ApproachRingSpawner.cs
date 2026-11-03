using UnityEngine;
using Vibranite.Feedback;
using Vibranite.Timing;

namespace Vibranite.RhythmGame
{
    /// <summary>
    /// Spawns one ApproachRing per beat, early enough that it finishes
    /// shrinking exactly on that beat.
    /// </summary>
    public class ApproachRingSpawner : MonoBehaviour
    {
        [SerializeField] private SongClock songClock;
        [SerializeField] private EffectPool ringPool;

        [Tooltip("What the rings center on, e.g. the lightstick tip.")]
        [SerializeField] private Transform ringAnchor;

        [Tooltip("How many beats each ring takes to shrink. Fractions allowed (1.5 = one and a half beats).")]
        [SerializeField, Min(0.25f)] private float leadBeats = 2f;

        private int nextTargetBeat;

        // ================= Unity lifecycle =================

        private void OnEnable()
        {
            songClock.SongStarted += ResetCueing;
            ResetCueing();
        }

        private void OnDisable()
        {
            songClock.SongStarted -= ResetCueing;
        }

        private void Update()
        {
            if (!songClock.IsPlaying) return;

            SkipBeatsAlreadyPassed();
            SpawnDueRings();
        }

        // ================= Cueing =================

        private void ResetCueing()
        {
            nextTargetBeat = 0;
        }

        private void SpawnDueRings()
        {
            // A loop, not an if: a frame hitch can make more than one ring due at once.
            while (IsRingDue(nextTargetBeat))
            {
                SpawnRing(nextTargetBeat);
                nextTargetBeat++;
            }
        }

        private void SkipBeatsAlreadyPassed()
        {
            // A ring whose beat is already over would appear and vanish in the same frame.
            while (nextTargetBeat <= songClock.CurrentBeat)
                nextTargetBeat++;
        }

        private bool IsRingDue(int targetBeat)
            => songClock.CurrentBeat >= targetBeat - leadBeats;

        private void SpawnRing(int targetBeat)
        {
            ApproachRing ring = ringPool.Spawn<ApproachRing>();
            if (ring != null) ring.Begin(songClock, targetBeat, leadBeats, ringAnchor);
        }
    }
}
