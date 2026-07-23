using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PickupItem : MonoBehaviour
{
    public enum ItemType { BloodChalice, Sunstone, BatSwarm, GothicKey }

    [SerializeField] private ItemType itemType;
    [SerializeField] private ParticleSystem collectParticles;
    [SerializeField] private AudioClip collectSound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && collectSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DraculaController dracula = other.GetComponent<DraculaController>();
        if (dracula == null) return;

        ApplyEffect(dracula);
        PlayEffects();
        Destroy(gameObject);
    }

    private void ApplyEffect(DraculaController dracula)
    {
        switch (itemType)
        {
            case ItemType.BloodChalice:
                dracula.AddJump(1);
                break;
            case ItemType.Sunstone:
                dracula.AddJump(-1);
                break;
            case ItemType.BatSwarm:
                dracula.MultiplyJumps(2);
                break;
            case ItemType.GothicKey:
                dracula.HasKey = true;
                break;
        }
    }

    private void PlayEffects()
    {
        if (collectSound != null && audioSource != null)
            audioSource.PlayOneShot(collectSound);

        if (collectParticles != null)
        {
            ParticleSystem ps = Instantiate(collectParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration);
        }
    }
}
