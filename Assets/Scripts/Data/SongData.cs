using UnityEngine;
using Vibranite.Timing;

namespace Vibranite.Data
{
    /// <summary>
    /// Everything the timing system needs to know about one piece of music.
    /// Swapping songs = swapping this asset. No code changes.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSongData", menuName = "Vibranite/Song Data")]
    public class SongData : ScriptableObject
    {
        [Header("Audio")]
        [SerializeField] private AudioClip clip;

        [Header("Tempo")]
        [SerializeField, Min(1f)] private float bpm = 120f;
        [SerializeField, Min(1)] private int beatsPerBar = 4;

        [Header("Alignment")]
        [Tooltip("Seconds from the start of the clip to the first beat (beat 0).")]
        [SerializeField, Min(0f)] private float firstBeatOffsetSeconds = 0f;

        public AudioClip Clip => clip;
        public float Bpm => bpm;
        public int BeatsPerBar => beatsPerBar;
        public double FirstBeatOffsetSeconds => firstBeatOffsetSeconds;
        public double SecondsPerBeat => BeatCalculator.SecondsPerBeat(bpm);
    }
}
