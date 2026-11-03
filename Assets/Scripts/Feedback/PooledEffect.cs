using UnityEngine;
using UnityEngine.Pool;

namespace Vibranite.Feedback
{
    /// <summary>
    /// Base class for any visual effect that lives in an EffectPool.
    /// Subclasses play their effect, then call ReturnToPool() when finished.
    /// </summary>
    public abstract class PooledEffect : MonoBehaviour
    {
        private IObjectPool<PooledEffect> ownerPool;
        private bool isInPool;

        // ---- Called by EffectPool ----

        internal void AssignPool(IObjectPool<PooledEffect> pool) => ownerPool = pool;
        internal void MarkTaken() => isInPool = false;
        internal void MarkReturned() => isInPool = true;

        // ---- Called by subclasses ----

        protected void ReturnToPool()
        {
            if (isInPool) return; // Already returned: releasing twice would corrupt the pool.

            if (ownerPool != null) ownerPool.Release(this);
            else Destroy(gameObject);
        }
    }
}
