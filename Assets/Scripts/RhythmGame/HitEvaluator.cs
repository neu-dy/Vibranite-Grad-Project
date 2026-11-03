using System;
using UnityEngine;
using Vibranite.Data;
using Vibranite.Detection;
using Vibranite.Timing;

namespace Vibranite.RhythmGame
{
    /// <summary>
    /// The bridge between input and grading. Listens to any number of HitSources,
    /// converts each hit's time into a beat position via the SongClock,
    /// asks the TimingJudge for a grade, and announces the HitResult.
    /// </summary>
    public class HitEvaluator : MonoBehaviour
    {
        [SerializeField] private SongClock songClock;
        [SerializeField] private TimingWindows timingWindows;
        [SerializeField] private HitSource[] hitSources;

        [Tooltip("Ignore hits that land closest to a beat before beat 0 (the count-in).")]
        [SerializeField] private bool ignoreCountInHits = true;

        public event Action<HitResult> HitJudged;

        private TimingJudge judge;

        // ================= Unity lifecycle =================

        private void Awake()
        {
            judge = new TimingJudge(timingWindows);
        }

        private void OnEnable()
        {
            SubscribeToSources();
        }

        private void OnDisable()
        {
            UnsubscribeFromSources();
        }

        // ================= Hit handling =================

        private void HandleHit(double hitDspTime)
        {
            if (!songClock.IsPlaying) return;

            double hitBeat = songClock.DspToBeat(hitDspTime);
            if (ShouldIgnore(hitBeat)) return;

            HitResult result = judge.Judge(hitBeat, songClock.SecondsPerBeat);
            HitJudged?.Invoke(result);
        }

        private bool ShouldIgnore(double hitBeat)
            => ignoreCountInHits && BeatCalculator.NearestBeat(hitBeat) < 0;

        // ================= Subscriptions =================

        private void SubscribeToSources()
        {
            foreach (HitSource source in hitSources)
                if (source != null) source.HitDetected += HandleHit;
        }

        private void UnsubscribeFromSources()
        {
            foreach (HitSource source in hitSources)
                if (source != null) source.HitDetected -= HandleHit;
        }
    }
}
