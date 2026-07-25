using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LightWindow : MonoBehaviour
{
    [Header("Size")]
    [SerializeField] private float width = 3f;
    [SerializeField] private float height = 5f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer lightBeam;

    [Header("Settings")]
    [SerializeField] private string failMessage = "Dracula was caught in the light!";

    private BoxCollider2D triggerCol;

    void OnValidate()
    {
        width = Mathf.Max(width, 0.1f);
        height = Mathf.Max(height, 0.1f);
        UpdateSize();
    }

    void Awake()
    {
        triggerCol = GetComponent<BoxCollider2D>();
        triggerCol.isTrigger = true;
        UpdateSize();
    }

    private void UpdateSize()
    {
        if (lightBeam == null) lightBeam = GetComponent<SpriteRenderer>();
        if (triggerCol == null) triggerCol = GetComponent<BoxCollider2D>();

        if (lightBeam != null)
        {
            lightBeam.drawMode = SpriteDrawMode.Tiled;
            lightBeam.size = new Vector2(width, height);
        }

        if (triggerCol != null)
            triggerCol.size = new Vector2(width, height);
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
