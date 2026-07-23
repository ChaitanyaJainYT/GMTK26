using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class HazardStake : MonoBehaviour
{
    [Header("Stake Settings")]
    [SerializeField] private int damageAmount = 1;

    [Header("Effects")]
    [SerializeField] private ParticleSystem impactParticles;
    [SerializeField] private AudioClip impactSound;

    public UnityEvent OnPlayerStaked;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && impactSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (impactSound != null && audioSource != null)
            audioSource.PlayOneShot(impactSound);

        if (impactParticles != null)
            impactParticles.Play();

        Debug.Log($"HazardStake [{name}]: Player staked — game over");
        OnPlayerStaked?.Invoke();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
    }
}
