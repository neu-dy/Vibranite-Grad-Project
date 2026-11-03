using System;
using UnityEngine;
using Vibranite.Data;

namespace Vibranite.Timing
{
    /// <summary>
    /// The song's clock. Plays a SongData clip on an exact audio-hardware time,
    /// reports where the song is (in seconds and beats), converts between
    /// time formats, and announces each beat and bar as it passes.
    ///
    /// It does NOT grade hits, open interaction windows, or drive visuals.
    /// Other scripts listen to it or ask it questions.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SongClock : MonoBehaviour
    {
        [Header("Song")]
        [SerializeField] private SongData songData;
        [SerializeField] private bool playOnStart = true;

        [Tooltip("Delay before playback so the audio system can start the clip on an exact time.")]
        [SerializeField, Min(0.05f)] private float scheduleLeadSeconds = 0.1f;

        [Header("Calibration")]
        [Tooltip("Hardware audio delay in seconds. Positive = the player hears audio later than Unity plays it.")]
        [SerializeField] private float audioLatencySeconds = 0f;

        [Header("Debug")]
        [SerializeField] private bool logBeats = false;

        // ---- Events ----

        /// <summary>Fires once per whole beat (0, 1, 2...), on the first frame after it passes.</summary>
        public event Action<int> BeatPassed;

        /// <summary>Fires on the first beat of each bar. Passes the bar index.</summary>
        public event Action<int> BarPassed;

        /// <summary>Fires when Play() schedules a song. Listeners reset their own beat tracking here.</summary>
        public event Action SongStarted;

        public event Action SongEnded;

        // ---- Public state ----

        public SongData SongData => songData;
        public bool IsPlaying { get; private set; }
        public double SecondsPerBeat => songData.SecondsPerBeat;

        /// <summary>Song position as the player hears it (latency-adjusted).</summary>
        public double SongSeconds => IsPlaying ? DspToSongSeconds(AudioSettings.dspTime) : 0.0;

        public double CurrentBeat => SongSecondsToBeat(SongSeconds);
        public double BeatPhase => BeatCalculator.BeatPhase(CurrentBeat);
        public double BeatsUntilNextBeat => BeatCalculator.BeatsUntilNextBeat(CurrentBeat);

        // ---- Private state ----

        private AudioSource audioSource;
        private double songStartDspTime;
        private int lastBeatAnnounced;

        // ================= Unity lifecycle =================

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            ConfigureAudioSource();
        }

        private void Start()
        {
            if (playOnStart) Play();
        }

        private void Update()
        {
            if (!IsPlaying) return;

            AnnounceCrossedBeats();
            CheckForSongEnd();
        }

        // ================= Playback controls =================

        public void Play(SongData newSong)
        {
            songData = newSong;
            Play();
        }

        public void Play()
        {
            if (!HasPlayableSong()) return;

            Stop();
            ScheduleClipStart();
            ResetBeatTracking();
            IsPlaying = true;
            SongStarted?.Invoke();
        }

        public void Stop()
        {
            audioSource.Stop();
            IsPlaying = false;
        }

        // ================= Time conversions =================
        // Hit sources record AudioSettings.dspTime; these turn it into song time.

        public double DspToSongSeconds(double dspTime)
            => dspTime - songStartDspTime - audioLatencySeconds;

        public double DspToBeat(double dspTime)
            => SongSecondsToBeat(DspToSongSeconds(dspTime));

        /// <summary>The dspTime at which the player will hear a given beat.</summary>
        public double BeatToDsp(double beat)
            => songStartDspTime + audioLatencySeconds
               + BeatCalculator.BeatToSeconds(beat, SecondsPerBeat, songData.FirstBeatOffsetSeconds);

        private double SongSecondsToBeat(double songSeconds)
            => BeatCalculator.SecondsToBeat(songSeconds, SecondsPerBeat, songData.FirstBeatOffsetSeconds);

        // ================= Internals =================

        private void ConfigureAudioSource()
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false; // Looping would restart the audio while the clock keeps counting.
        }

        private bool HasPlayableSong()
        {
            if (songData != null && songData.Clip != null) return true;

            Debug.LogError($"[SongClock] No SongData or clip assigned on {name}.", this);
            return false;
        }

        private void ScheduleClipStart()
        {
            audioSource.clip = songData.Clip;
            songStartDspTime = AudioSettings.dspTime + scheduleLeadSeconds;
            audioSource.PlayScheduled(songStartDspTime);
        }

        private void ResetBeatTracking()
        {
            // Beat 0 is the first one announced. Count-in beats (negative) stay silent.
            lastBeatAnnounced = -1;
        }

        private void AnnounceCrossedBeats()
        {
            int currentWholeBeat = (int)Math.Floor(CurrentBeat);

            // A loop, not an if: after a frame hitch, no beat gets skipped.
            while (lastBeatAnnounced < currentWholeBeat)
            {
                lastBeatAnnounced++;
                AnnounceBeat(lastBeatAnnounced);
            }
        }

        private void AnnounceBeat(int beat)
        {
            if (logBeats) Debug.Log($"[SongClock] Beat {beat}");

            BeatPassed?.Invoke(beat);

            if (IsFirstBeatOfBar(beat))
                BarPassed?.Invoke(beat / songData.BeatsPerBar);
        }

        private bool IsFirstBeatOfBar(int beat)
            => beat % songData.BeatsPerBar == 0;

        private void CheckForSongEnd()
        {
            double audioSeconds = AudioSettings.dspTime - songStartDspTime;
            if (audioSeconds < songData.Clip.length) return;

            Stop();
            SongEnded?.Invoke();
        }
    }
}