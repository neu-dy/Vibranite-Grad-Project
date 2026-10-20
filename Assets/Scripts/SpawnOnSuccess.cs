using UnityEngine;
using UnityEngine.Pool;

/// Spawns a pooled circle wherever a shake succeeds.
public class SpawnOnSuccess : MonoBehaviour
{
    [SerializeField] private ShakeDetection detector;
    [SerializeField] private CircleFeedback circlePrefab;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 30;

    private ObjectPool<CircleFeedback> pool;

    private void Awake()
    {
        pool = new ObjectPool<CircleFeedback>(
            CreateCircle,
            c => c.gameObject.SetActive(true),
            c => c.gameObject.SetActive(false),
            c => Destroy(c.gameObject),
            true, defaultCapacity, maxSize);
    }

    private void OnEnable() => detector.ShakeDetected += Spawn;
    private void OnDisable() => detector.ShakeDetected -= Spawn;

    private CircleFeedback CreateCircle()
    {
        CircleFeedback c = Instantiate(circlePrefab);
        c.Init(pool);
        return c;
    }

    private void Spawn(Vector3 position) => pool.Get().Play(position);
}