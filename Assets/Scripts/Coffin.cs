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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DraculaController dracula = other.GetComponent<DraculaController>();
        if (dracula == null) return;

        if (isUnlocked)
        {
            Win();
            return;
        }

        if (requiresKey && !dracula.HasKey)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.FailLevel("Locked Coffin! Find the Gothic Key.");
            return;
        }

        Unlock();
        Win();
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
        if (GameManager.Instance != null)
            GameManager.Instance.WinLevel();
    }

    private void UpdateSprite()
    {
        if (coffinSprite == null) return;
        coffinSprite.sprite = isUnlocked ? unlockedSprite : lockedSprite;
    }
}
