using System;

namespace Vibranite.Timing
{
    /// <summary>
    /// Pure beat/time conversions. No Unity types, no state:
    /// the same input always gives the same output.
    ///
    /// A "beat" is a double: 3.0 = exactly on beat 3, 3.5 = halfway to beat 4.
    /// Negative beats = before the first beat of the song.
    /// </summary>
    public static class BeatCalculator
    {
        private const double SecondsPerMinute = 60.0;

        // ---- Tempo ----

        public static double SecondsPerBeat(double bpm)
            => SecondsPerMinute / bpm;

        // ---- Seconds <-> Beats ----

        public static double SecondsToBeat(double songSeconds, double secondsPerBeat, double firstBeatOffsetSeconds)
            => (songSeconds - firstBeatOffsetSeconds) / secondsPerBeat;

        public static double BeatToSeconds(double beat, double secondsPerBeat, double firstBeatOffsetSeconds)
            => firstBeatOffsetSeconds + beat * secondsPerBeat;

        // ---- Position within the beat grid ----

        /// <summary>Closest whole beat. 3.4 -> 3, 3.6 -> 4.</summary>
        public static double NearestBeat(double beat)
            => Math.Floor(beat + 0.5);

        /// <summary>The next whole beat strictly ahead. 3.0 -> 4, 3.2 -> 4.</summary>
        public static double NextBeat(double beat)
            => Math.Floor(beat) + 1.0;

        /// <summary>How far through the current beat, 0 (on the beat) to just under 1.</summary>
        public static double BeatPhase(double beat)
            => beat - Math.Floor(beat);

        public static double BeatsUntilNextBeat(double beat)
            => NextBeat(beat) - beat;

        /// <summary>
        /// Signed distance to the nearest beat, in seconds.
        /// Negative = early, positive = late. This is what the TimingJudge grades.
        /// </summary>
        public static double OffsetFromNearestBeatSeconds(double beat, double secondsPerBeat)
            => (beat - NearestBeat(beat)) * secondsPerBeat;
    }
}
