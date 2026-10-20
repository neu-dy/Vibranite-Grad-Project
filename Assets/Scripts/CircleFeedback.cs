using UnityEngine;
using UnityEngine.Pool;

/// Expands and fades out, then returns itself to the pool.
/// Prefab: Sphere (reads as a circle from any angle), collider REMOVED,
/// material = URP Unlit, Surface Type: Transparent.
[RequireComponent(typeof(Renderer))]
public class CircleFeedback : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.4f;
    [SerializeField] private float startScale = 0.05f;
    [SerializeField] private float endScale = 0.4f;
    [SerializeField] private Color color = Color.cyan;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private IObjectPool<CircleFeedback> pool;
    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private float elapsed;

    public void Init(IObjectPool<CircleFeedback> owner) => pool = owner;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void Play(Vector3 position)
    {
        transform.position = position;
        elapsed = 0f;
        Apply(0f);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);
        Apply(t);
        if (t >= 1f) Release();
    }

    private void Apply(float t)
    {
        transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
        Color c = color;
        c.a = 1f - t;
        mpb.SetColor(BaseColorId, c);
        rend.SetPropertyBlock(mpb);
    }

    private void Release()
    {
        if (pool != null) pool.Release(this);
        else Destroy(gameObject);
    }
}