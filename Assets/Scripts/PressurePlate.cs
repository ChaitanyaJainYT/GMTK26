using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PressurePlate : MonoBehaviour
{
    [SerializeField] private bool isPressed;

    [Header("Target")]
    [SerializeField] private LightWindow targetWindow;

    [Header("Window Visual")]
    [SerializeField] private SpriteRenderer windowSprite;
    [SerializeField] private Sprite windowOpenSprite;
    [SerializeField] private Sprite windowClosedSprite;

    [Header("Plate Visual")]
    [SerializeField] private SpriteRenderer plateSprite;
    [SerializeField] private Color pressedColor = new Color(0.6f, 0.6f, 0.6f);
    [SerializeField] private Color releasedColor = Color.white;

    private BoxCollider2D triggerCol;

    private void UpdateVisuals()
    {
        if (targetWindow != null)
            targetWindow.SetActive(!isPressed);

        if (windowSprite != null)
            windowSprite.sprite = isPressed ? windowClosedSprite : windowOpenSprite;

        if (plateSprite != null)
            plateSprite.color = isPressed ? pressedColor : releasedColor;
    }

    void OnValidate()
    {
        UpdateVisuals();
    }

    void Awake()
    {
        triggerCol = GetComponent<BoxCollider2D>();
        triggerCol.isTrigger = true;
        UpdateVisuals();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPressed = !isPressed;
        UpdateVisuals();
    }
}
