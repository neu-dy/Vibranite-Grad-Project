namespace Vibranite.RhythmGame
{
    /// <summary>
    /// The outcome of judging one hit. Immutable: once a hit is judged,
    /// nothing downstream can change what happened.
    /// </summary>
    public readonly struct HitResult
    {
        public HitGrade Grade { get; }

        /// <summary>The whole beat this hit was judged against.</summary>
        public int TargetBeat { get; }

        /// <summary>Exact beat position of the hit, e.g. 11.96.</summary>
        public double HitBeat { get; }

        /// <summary>Signed distance from the target beat. Negative = early, positive = late.</summary>
        public double OffsetSeconds { get; }

        public HitResult(HitGrade grade, int targetBeat, double hitBeat, double offsetSeconds)
        {
            Grade = grade;
            TargetBeat = targetBeat;
            HitBeat = hitBeat;
            OffsetSeconds = offsetSeconds;
        }

        public bool IsSuccess => Grade >= HitGrade.Good;
        public bool IsEarly => OffsetSeconds < 0.0;
        public double OffsetMilliseconds => OffsetSeconds * 1000.0;

        /// <summary>Readable log line, e.g. "Perfect on beat 12 (-18 ms, early)".</summary>
        public override string ToString()
        {
            string direction = IsEarly ? "early" : "late";
            return $"{Grade} on beat {TargetBeat} ({OffsetMilliseconds:+0;-0} ms, {direction})";
        }
    }
}
