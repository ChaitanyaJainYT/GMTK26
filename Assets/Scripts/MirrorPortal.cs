using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MirrorPortal : MonoBehaviour
{
    [Header("Link")]
    [SerializeField] private MirrorPortal otherPortal;

    [Header("Effects")]
    [SerializeField] private ParticleSystem warpParticles;
    [SerializeField] private AudioClip warpSound;

    public event System.Action OnWarpped;

    private AudioSource audioSource;
    private bool playerInside;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && warpSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerInside) return;
        if (otherPortal == null) return;

        DraculaController dracula = other.GetComponent<DraculaController>();
        if (dracula == null) return;

        Vector3 warpPos = otherPortal.transform.position;
        Platform platform = otherPortal.GetComponentInParent<Platform>();
        GameObject platformObj = platform != null ? platform.gameObject : null;

        dracula.WarpTo(warpPos, platformObj);
        otherPortal.playerInside = true;

        OnWarpped?.Invoke();

        Debug.Log($"MirrorPortal: Teleported Dracula from {name} → {otherPortal.name}");

        if (warpSound != null && audioSource != null)
            audioSource.PlayOneShot(warpSound);

        if (warpParticles != null)
            warpParticles.Play();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
    }

    void OnDrawGizmos()
    {
        if (otherPortal != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, otherPortal.transform.position);
        }
    }
}
