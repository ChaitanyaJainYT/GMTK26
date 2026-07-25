using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PickupItem : MonoBehaviour
{
    public enum ItemType { BloodChalice, Sunstone, BatSwarm, GothicKey }

    [SerializeField] private ItemType itemType;
    [SerializeField] private ParticleSystem collectParticles;
    [SerializeField] private AudioClip collectSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DraculaController dracula = other.GetComponent<DraculaController>();
        if (dracula == null) return;

        ApplyEffect(dracula);
        PlayEffects(dracula);
        Destroy(gameObject);
    }

    private void ApplyEffect(DraculaController dracula)
    {
        switch (itemType)
        {
            case ItemType.BloodChalice:
                dracula.AddJump(1);
                Debug.Log($"PickupItem [{name}]: BloodChalice collected — +1 jump");
                break;
            case ItemType.Sunstone:
                dracula.AddJump(-1);
                Debug.Log($"PickupItem [{name}]: Sunstone collected — -1 jump");
                break;
            case ItemType.BatSwarm:
                dracula.MultiplyJumps(2);
                Debug.Log($"PickupItem [{name}]: BatSwarm collected — jumps doubled");
                break;
            case ItemType.GothicKey:
                dracula.HasKey = true;
                Debug.Log($"PickupItem [{name}]: GothicKey collected — hasKey = true");
                break;
        }
    }

    private void PlayEffects(DraculaController dracula)
    {
        if (collectSound != null)
        {
            AudioSource playerAudio = dracula.GetComponent<AudioSource>();
            if (playerAudio != null)
                playerAudio.PlayOneShot(collectSound);
        }

        if (collectParticles != null)
        {
            ParticleSystem ps = Instantiate(collectParticles, transform.position, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration);
        }
    }
}
