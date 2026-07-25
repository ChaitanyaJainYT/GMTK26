using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LightWindow : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer lightBeam;

    [Header("Settings")]
    [SerializeField] private string failMessage = "Dracula was caught in the light!";

    private BoxCollider2D triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<BoxCollider2D>();
        triggerCol.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"LightWindow [{name}]: {failMessage}");
        if (GameManager.Instance != null)
            GameManager.Instance.FailLevel(failMessage);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
