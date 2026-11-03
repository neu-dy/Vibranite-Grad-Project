using System;
using Vibranite.Data;
using Vibranite.Timing;

namespace Vibranite.RhythmGame
{
    /// <summary>
    /// Grades a hit against the beat grid. Pure logic: it never asks where the
    /// hit came from (drum, keyboard, collision) or where the time came from.
    /// Give it a beat position, get a HitResult back.
    /// </summary>
    public class TimingJudge
    {
        private readonly TimingWindows windows;

        public TimingJudge(TimingWindows windows)
        {
            this.windows = windows;
        }

        public HitResult Judge(double hitBeat, double secondsPerBeat)
        {
            int targetBeat = (int)BeatCalculator.NearestBeat(hitBeat);
            double offsetSeconds = BeatCalculator.OffsetFromNearestBeatSeconds(hitBeat, secondsPerBeat);
            HitGrade grade = GradeForOffset(offsetSeconds);

            return new HitResult(grade, targetBeat, hitBeat, offsetSeconds);
        }

        private HitGrade GradeForOffset(double offsetSeconds)
        {
            double distanceFromBeat = Math.Abs(offsetSeconds);

            if (distanceFromBeat <= windows.PerfectWindowSeconds) return HitGrade.Perfect;
            if (distanceFromBeat <= windows.GoodWindowSeconds) return HitGrade.Good;
            return HitGrade.Miss;
        }
    }
}
