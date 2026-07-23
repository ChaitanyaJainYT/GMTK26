using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProceduralDracula2D : MonoBehaviour
{
    [Header("Sprite Canvas Dimensions")]
    public int textureWidth = 32;
    public int textureHeight = 40;
    public float pixelsPerUnit = 16f; // Controls game scale

    [Header("Colors")]
    public Color capeColor = new Color(0.53f, 0.07f, 0.21f);  // #881337 Crimson
    public Color suitColor = new Color(0.06f, 0.09f, 0.16f);  // #0f172a Midnight Dark
    public Color collarColor = new Color(0.74f, 0.07f, 0.23f);// #be123c Bright Red
    public Color skinColor = new Color(0.94f, 0.96f, 0.97f);  // #f1f5f9 Pale Marble
    public Color eyeColor = new Color(0.93f, 0.26f, 0.26f);   // #ef4444 Glowing Red

    [Header("Animation Settings")]
    public float waveSpeed = 8f;
    public float waveAmount = 3f;

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

        // Determine cape billow based on movement or time
        float vx = rb != null ? Mathf.Abs(rb.linearVelocity.x) : 0f;
        float animationTime = Time.time * waveSpeed;
        float currentWave = Mathf.Sin(animationTime) * waveAmount * (1f + vx * 0.2f);

        // --- LAYER 1: Flowing Cape (Back) ---
        int capeTopY = 24;
        int capeBottomY = 2;
        int baseLeftX = 6;
        int baseRightX = 25;

        for (int y = capeBottomY; y <= capeTopY; y++)
        {
            // Calculate cape flare outwards towards bottom + wave offset
            float progress = 1f - ((float)(y - capeBottomY) / (capeTopY - capeBottomY));
            int flare = Mathf.RoundToInt(progress * (5f + Mathf.Abs(currentWave) * 0.5f));
            int waveX = Mathf.RoundToInt(currentWave * progress);

            int startX = Mathf.Clamp(baseLeftX - flare + waveX, 0, textureWidth - 1);
            int endX = Mathf.Clamp(baseRightX + flare + waveX, 0, textureWidth - 1);

            for (int x = startX; x <= endX; x++)
            {
                SetPixel(x, y, capeColor);
            }
        }

        // --- LAYER 2: Vampiric Suit / Body ---
        int suitLeft = 10;
        int suitRight = 21;
        int suitBottom = 4;
        int suitTop = 22;

        for (int y = suitBottom; y <= suitTop; y++)
        {
            for (int x = suitLeft; x <= suitRight; x++)
            {
                SetPixel(x, y, suitColor);
            }
        }

        // --- LAYER 3: High Gothic Collar ---
        SetPixel(8, 28, collarColor); SetPixel(9, 28, collarColor);
        SetPixel(22, 28, collarColor); SetPixel(23, 28, collarColor);
        for (int y = 22; y <= 27; y++)
        {
            SetPixel(9, y, collarColor);
            SetPixel(10, y, collarColor);
            SetPixel(21, y, collarColor);
            SetPixel(22, y, collarColor);
        }

        // --- LAYER 4: Pale Face ---
        int faceLeft = 11;
        int faceRight = 20;
        int faceBottom = 23;
        int faceTop = 30;

        for (int y = faceBottom; y <= faceTop; y++)
        {
            for (int x = faceLeft; x <= faceRight; x++)
            {
                SetPixel(x, y, skinColor);
            }
        }

        // --- LAYER 5: Directional Glowing Red Eyes ---
        int eyeXOffset = 0;
        if (rb != null)
        {
            if (rb.linearVelocity.x < -0.1f) eyeXOffset = -1;
            else if (rb.linearVelocity.x > 0.1f) eyeXOffset = 1;
        }

        // Left Eye
        SetPixel(13 + eyeXOffset, 26, eyeColor);
        SetPixel(13 + eyeXOffset, 27, eyeColor);

        // Right Eye
        SetPixel(18 + eyeXOffset, 26, eyeColor);
        SetPixel(18 + eyeXOffset, 27, eyeColor);

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
