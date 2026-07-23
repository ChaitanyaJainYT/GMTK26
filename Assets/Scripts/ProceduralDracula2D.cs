using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProceduralDracula2D : MonoBehaviour
{
    [Header("Sprite Canvas Dimensions")]
    public int textureWidth = 8;
    public int textureHeight = 8;
    public float pixelsPerUnit = 8f;

    [Header("Colors")]
    public Color capeColor = new Color(0.53f, 0.07f, 0.21f);
    public Color suitColor = new Color(0.06f, 0.09f, 0.16f);
    public Color collarColor = new Color(0.74f, 0.07f, 0.23f);
    public Color skinColor = new Color(0.94f, 0.96f, 0.97f);
    public Color eyeColor = new Color(0.93f, 0.26f, 0.26f);

    [Header("Animation Settings")]
    public float waveSpeed = 8f;
    public float waveAmount = 1f;

    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    private Color32[] pixels;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // 1. Create a blank dynamic texture
        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // Crisp Pixel-Art Style
        pixels = new Color32[textureWidth * textureHeight];

        // 2. Wrap it into a Unity Sprite
        Sprite newSprite = Sprite.Create(
            texture,
            new Rect(0, 0, textureWidth, textureHeight),
            new Vector2(0.5f, 0f), // Pivot at feet bottom
            pixelsPerUnit
        );
        spriteRenderer.sprite = newSprite;
    }

    void Update()
    {
        // Redraw Dracula on canvas every frame for animation
        DrawDraculaOnCanvas();
    }

    void DrawDraculaOnCanvas()
    {
        // Clear background (Transparent)
        Color32 clearColor = new Color32(0, 0, 0, 0);
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clearColor;
        }

        float vx = rb != null ? Mathf.Abs(rb.linearVelocity.x) : 0f;
        float animationTime = Time.time * waveSpeed;
        float currentWave = Mathf.Sin(animationTime) * waveAmount * (1f + vx * 0.2f);

        // LAYER 1: Flowing Cape (fills behind body with animated sway)
        int capeTopY = 6;
        int capeBottomY = 0;
        int baseLeftX = 2;
        int baseRightX = 6;

        for (int y = capeBottomY; y <= capeTopY; y++)
        {
            float progress = 1f - ((float)(y - capeBottomY) / (capeTopY - capeBottomY));
            int flare = Mathf.RoundToInt(progress * (1f + Mathf.Abs(currentWave) * 0.15f));
            int waveX = Mathf.RoundToInt(currentWave * progress * 0.25f);

            int startX = Mathf.Clamp(baseLeftX - flare + waveX, 0, textureWidth - 1);
            int endX = Mathf.Clamp(baseRightX + flare + waveX, 0, textureWidth - 1);

            for (int x = startX; x <= endX; x++)
            {
                SetPixel(x, y, capeColor);
            }
        }

        // LAYER 2: Vampiric Suit / Body
        for (int y = 1; y <= 4; y++)
        {
            for (int x = 2; x <= 5; x++)
            {
                SetPixel(x, y, suitColor);
            }
        }

        // LAYER 3: High Gothic Collar
        SetPixel(2, 6, collarColor);
        SetPixel(6, 6, collarColor);
        for (int y = 4; y <= 5; y++)
        {
            SetPixel(2, y, collarColor);
            SetPixel(3, y, collarColor);
            SetPixel(5, y, collarColor);
            SetPixel(6, y, collarColor);
        }

        // LAYER 4: Pale Face
        int faceLeft = 3;
        int faceRight = 5;
        int faceBottom = 5;
        int faceTop = 6;

        for (int y = faceBottom; y <= faceTop; y++)
        {
            for (int x = faceLeft; x <= faceRight; x++)
            {
                SetPixel(x, y, skinColor);
            }
        }

        // LAYER 5: Directional Glowing Red Eyes
        int eyeXOffset = 0;
        if (rb != null)
        {
            if (rb.linearVelocity.x < -0.1f) eyeXOffset = -1;
            else if (rb.linearVelocity.x > 0.1f) eyeXOffset = 1;
        }

        SetPixel(3 + eyeXOffset, 5, eyeColor);
        SetPixel(5 + eyeXOffset, 5, eyeColor);

        // Apply pixels to Unity Texture
        texture.SetPixels32(pixels);
        texture.Apply();
    }

    // Helper method to draw pixels safely inside bounds
    void SetPixel(int x, int y, Color color)
    {
        if (x >= 0 && x < textureWidth && y >= 0 && y < textureHeight)
        {
            pixels[y * textureWidth + x] = color;
        }
    }
}
