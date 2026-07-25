using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Coffin : MonoBehaviour
{
    [Header("Lock Settings")]
    [SerializeField] private bool requiresKey = true;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer coffinSprite;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;

    [Header("Effects")]
    [SerializeField] private ParticleSystem unlockParticles;
    [SerializeField] private AudioClip unlockSound;

    private BoxCollider2D triggerCol;
    private AudioSource audioSource;
    private bool isUnlocked;
    private DraculaController dracula;
    private bool isTriggered = false;

    void OnValidate()
    {
        isUnlocked = !requiresKey;
        UpdateSprite();
    }

    void Awake()
    {
        triggerCol = GetComponent<BoxCollider2D>();
        triggerCol.isTrigger = true;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && unlockSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
        UpdateSprite();
    }

    void Start()
    {
        dracula = FindObjectOfType<DraculaController>();
        if (dracula != null)
            dracula.OnHasKeyChanged += HandleHasKeyChanged;
    }

    void OnDestroy()
    {
        if (dracula != null)
            dracula.OnHasKeyChanged -= HandleHasKeyChanged;
    }

    private void HandleHasKeyChanged(bool hasKey)
    {
        if (!isUnlocked && requiresKey && hasKey)
            Unlock();
    }

    private void triggered(Collider2D other)
    {
        if (isTriggered) return;

        if (!other.CompareTag("Player")) return;

        DraculaController dracula = other.GetComponent<DraculaController>();
        if (dracula == null) return;

        if (isUnlocked)
        {
            TryWin(dracula);
            return;
        }

        if (requiresKey && !dracula.HasKey)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.FailLevel("Locked Coffin! Find the Gothic Key.");
            return;
        }

        Unlock();
        TryWin(dracula);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        triggered(other);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        triggered(collision);
    }

    private void TryWin(DraculaController dracula)
    {
        if (!dracula.IsGrounded)
            return;

        if (dracula.RemainingJumps == 0)
            Win();
        else if (GameManager.Instance != null)
            GameManager.Instance.FailLevel("Overload! Sun burns Dracula!");
    }

    private void Unlock()
    {
        isUnlocked = true;
        UpdateSprite();

        if (unlockSound != null && audioSource != null)
            audioSource.PlayOneShot(unlockSound);

        if (unlockParticles != null)
            unlockParticles.Play();
    }

    private void Win()
    {
        isTriggered = true;
        if (GameManager.Instance != null)
            GameManager.Instance.WinLevel();
    }

    private void UpdateSprite()
    {
        if (coffinSprite == null) return;
        coffinSprite.sprite = isUnlocked ? unlockedSprite : lockedSprite;
    }
}
