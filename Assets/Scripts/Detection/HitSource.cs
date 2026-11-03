using System;
using UnityEngine;

namespace Vibranite.Detection
{
    /// <summary>
    /// Base class for anything that can produce a hit: keyboard, collision, drum, etc.
    /// Subclasses only decide WHEN a hit happens and call ReportHit().
    /// Listeners receive the dspTime of the hit and never need to know the source.
    /// </summary>
    public abstract class HitSource : MonoBehaviour
    {
        /// <summary>Fires with the AudioSettings.dspTime at the moment of the hit.</summary>
        public event Action<double> HitDetected;

        protected void ReportHit()
            => ReportHit(AudioSettings.dspTime);

        protected void ReportHit(double dspTime)
            => HitDetected?.Invoke(dspTime);
    }
}
