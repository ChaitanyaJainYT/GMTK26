using UnityEngine;
using UnityEngine.Events;

public class Platform : MonoBehaviour
{
    private static int nextId = 1;

    public enum PlatformType { Standard, Start, Crumble, GoalCrypt }

    [Header("Identity")]
    public PlatformType type = PlatformType.Standard;
    public int platformId;
    public bool requiresKey = false;

    [Header("Size")]
    [SerializeField] private float width = 4f;
    [SerializeField] private float height = 1f;

    [Header("Crumble Settings")]
    [SerializeField] private float crumbleDelay = 0.8f;
    [SerializeField] private float shakeIntensity = 0.08f;
    [SerializeField] private float shakeFrequency = 30f;
    [SerializeField] private ParticleSystem crumbleParticles;
    [SerializeField] private AudioClip crumbleSound;

    public UnityEvent onCrumble;

    private SpriteRenderer sr;
    private BoxCollider2D boxCol;
    private AudioSource audioSource;
    private bool isCrumbleTriggered;

    void OnValidate()
    {
        width = Mathf.Max(width, 0.1f);
        height = Mathf.Max(height, 0.1f);
        UpdateSize();
    }

    private void UpdateSize()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (boxCol == null) boxCol = GetComponent<BoxCollider2D>();

        if (sr != null)
        {
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(width, height);
        }

        if (boxCol != null)
            boxCol.size = new Vector2(width, height);
    }

    void Awake()
    {
        if (platformId == 0)
            platformId = nextId++;
        sr = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        UpdateSize();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && crumbleSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (type != PlatformType.Crumble || isCrumbleTriggered) return;

        Debug.Log($"Platform [{platformId}]: Crumble triggered by Dracula");
        StartCoroutine(CrumbleRoutine());
    }

    private System.Collections.IEnumerator CrumbleRoutine()
    {
        isCrumbleTriggered = true;
        float timer = crumbleDelay;
        Vector3 originalPos = transform.localPosition;

        if (crumbleSound != null && audioSource != null)
            audioSource.PlayOneShot(crumbleSound);

        while (timer > 0f)
        {
            float offsetX = Mathf.Sin(timer * shakeFrequency) * shakeIntensity;
            transform.localPosition = originalPos + new Vector3(offsetX, 0f, 0f);
            timer -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        boxCol.enabled = false;

        Debug.Log($"Platform [{platformId}]: Crumble complete — collider disabled");

        if (crumbleParticles != null)
            crumbleParticles.Play();

        this.gameObject.SetActive(false);
        onCrumble?.Invoke();
    }
}
