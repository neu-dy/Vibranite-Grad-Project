using UnityEngine;

namespace Vibranite.Data
{
    /// <summary>
    /// How forgiving the timing is. Each value is a half-width:
    /// 70 ms Perfect = anywhere from 70 ms early to 70 ms late.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTimingWindows", menuName = "Vibranite/Timing Windows")]
    public class TimingWindows : ScriptableObject
    {
        [Tooltip("± milliseconds from the beat that count as Perfect.")]
        [SerializeField, Min(1f)] private float perfectWindowMs = 70f;

        [Tooltip("± milliseconds from the beat that count as Good. Anything outside = Miss.")]
        [SerializeField, Min(1f)] private float goodWindowMs = 150f;

        public double PerfectWindowSeconds => perfectWindowMs / 1000.0;
        public double GoodWindowSeconds => goodWindowMs / 1000.0;

        private void OnValidate()
        {
            // Good must always be at least as wide as Perfect.
            goodWindowMs = Mathf.Max(goodWindowMs, perfectWindowMs);
        }
    }
}
