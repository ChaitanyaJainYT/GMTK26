using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PressurePlate : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private LightWindow targetWindow;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer plateSprite;
    [SerializeField] private Color pressedColor = new Color(0.6f, 0.6f, 0.6f);
    [SerializeField] private Color releasedColor = Color.white;

    private BoxCollider2D triggerCol;
    private bool isPressed;

    void Awake()
    {
        triggerCol = GetComponent<BoxCollider2D>();
        triggerCol.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPressed = !isPressed;

        if (targetWindow != null)
            targetWindow.SetActive(!isPressed);

        if (plateSprite != null)
            plateSprite.color = isPressed ? pressedColor : releasedColor;
    }
}
