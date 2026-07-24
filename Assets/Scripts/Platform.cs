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

    [Header("Crumble Settings")]
    [SerializeField] private float crumbleDelay = 0.8f;
    [SerializeField] private float shakeIntensity = 0.08f;
    [SerializeField] private float shakeFrequency = 30f;
    [SerializeField] private ParticleSystem crumbleParticles;
    [SerializeField] private AudioClip crumbleSound;

    public UnityEvent onCrumble;

    private Collider2D col;
    private AudioSource audioSource;
    private bool isCrumbleTriggered;

    void Awake()
    {
        if (platformId == 0)
            platformId = nextId++;
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && crumbleSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (type != PlatformType.Crumble || isCrumbleTriggered) return;

        DraculaController dracula = collision.gameObject.GetComponent<DraculaController>();
        if (dracula != null && dracula.RemainingJumps > 0)
        {
            Debug.Log($"Platform [{platformId}]: Crumble triggered by Dracula");
            StartCoroutine(CrumbleRoutine());
        }
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
        col.enabled = false;

        Debug.Log($"Platform [{platformId}]: Crumble complete — collider disabled");

        if (crumbleParticles != null)
            crumbleParticles.Play();

        this.gameObject.SetActive(false);
        onCrumble?.Invoke();
    }
}
