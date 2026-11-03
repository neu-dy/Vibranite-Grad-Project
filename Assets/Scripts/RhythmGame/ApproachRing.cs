using UnityEngine;
using Vibranite.Feedback;
using Vibranite.Timing;

namespace Vibranite.RhythmGame
{
    /// <summary>
    /// A timing cue that shrinks and fades in, reaching its smallest size
    /// exactly on its target beat. Driven by the SongClock every frame,
    /// so it can never drift out of sync with the music.
    ///
    /// Prefab: Sphere, collider REMOVED, URP Unlit material set to Transparent.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class ApproachRing : PooledEffect
    {
        [SerializeField] private float startScale = 0.4f;
        [SerializeField] private float endScale = 0.05f;
        [SerializeField] private Color color = Color.cyan;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private Renderer ringRenderer;
        private MaterialPropertyBlock propertyBlock;

        private SongClock songClock;
        private Transform followTarget;
        private double targetBeat;
        private double leadBeats;

        // ================= Unity lifecycle =================

        private void Awake()
        {
            ringRenderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            FollowTarget();
            ApplyProgress(CalculateProgress());

            if (HasReachedTargetBeat()) ReturnToPool();
        }

        // ================= Public API =================

        /// <summary>Starts this ring for one beat. Called by the ApproachRingSpawner.</summary>
        public void Begin(SongClock clock, double beat, double lead, Transform follow)
        {
            songClock = clock;
            targetBeat = beat;
            leadBeats = lead;
            followTarget = follow;

            FollowTarget();
            ApplyProgress(CalculateProgress());
        }

        // ================= Internals =================

        /// <summary>0 when the ring appears, 1 exactly on the target beat.</summary>
        private float CalculateProgress()
        {
            double beatsRemaining = targetBeat - songClock.CurrentBeat;
            return Mathf.Clamp01(1f - (float)(beatsRemaining / leadBeats));
        }

        private void ApplyProgress(float progress)
        {
            ApplyScale(progress);
            ApplyAlpha(progress);
        }

        private void ApplyScale(float progress)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, progress);
        }

        private void ApplyAlpha(float progress)
        {
            Color faded = color;
            faded.a = progress; // Fades IN as it closes, the reverse of the old expanding circle.

            propertyBlock.SetColor(BaseColorId, faded);
            ringRenderer.SetPropertyBlock(propertyBlock);
        }

        private void FollowTarget()
        {
            if (followTarget != null) transform.position = followTarget.position;
        }

        private bool HasReachedTargetBeat()
            => songClock.CurrentBeat >= targetBeat;
    }
}
