namespace Vibranite.RhythmGame
{
    /// <summary>
    /// How well a hit landed. Ordered worst -> best on purpose,
    /// so "grade >= HitGrade.Good" reads as "was it a success?"
    /// </summary>
    public enum HitGrade
    {
        Miss = 0,
        Good = 1,
        Perfect = 2
    }
}
