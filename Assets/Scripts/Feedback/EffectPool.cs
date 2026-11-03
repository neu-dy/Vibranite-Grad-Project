using UnityEngine;
using UnityEngine.Pool;

namespace Vibranite.Feedback
{
    /// <summary>
    /// Reusable pool for one effect prefab. Spawning reuses an inactive copy
    /// instead of Instantiate/Destroy every time, which avoids frame hitches.
    /// One EffectPool per effect type (rings, hit sparks, etc.).
    /// </summary>
    public class EffectPool : MonoBehaviour
    {
        [SerializeField] private PooledEffect prefab;
        [SerializeField, Min(1)] private int defaultCapacity = 10;
        [SerializeField, Min(1)] private int maxSize = 50;

        private ObjectPool<PooledEffect> pool;

        // ================= Unity lifecycle =================

        private void Awake()
        {
            pool = CreatePool();
        }

        // ================= Public API =================

        /// <summary>Takes an effect from the pool as its specific type, e.g. Spawn&lt;ApproachRing&gt;().</summary>
        public T Spawn<T>() where T : PooledEffect
        {
            PooledEffect effect = pool.Get();
            if (effect is T typedEffect) return typedEffect;

            Debug.LogError($"[EffectPool] Prefab on {name} is not a {typeof(T).Name}.", this);
            pool.Release(effect);
            return null;
        }

        // ================= Pool callbacks =================

        private ObjectPool<PooledEffect> CreatePool()
            => new ObjectPool<PooledEffect>(
                CreateEffect, OnTakeFromPool, OnReturnToPool, OnDestroyEffect,
                true, defaultCapacity, maxSize);

        private PooledEffect CreateEffect()
        {
            PooledEffect effect = Instantiate(prefab, transform); // Parented here to keep the Hierarchy tidy.
            effect.AssignPool(pool);
            return effect;
        }

        private void OnTakeFromPool(PooledEffect effect)
        {
            effect.MarkTaken();
            effect.gameObject.SetActive(true);
        }

        private void OnReturnToPool(PooledEffect effect)
        {
            effect.MarkReturned();
            effect.gameObject.SetActive(false);
        }

        private void OnDestroyEffect(PooledEffect effect)
        {
            Destroy(effect.gameObject);
        }
    }
}
