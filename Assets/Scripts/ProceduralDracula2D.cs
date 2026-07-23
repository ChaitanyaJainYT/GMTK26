using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProceduralDracula2D : MonoBehaviour
{
    [Header("Sprite Canvas Dimensions")]
    public int textureWidth = 16;
    public int textureHeight = 16;
    public float pixelsPerUnit = 16f; // Standard 16x16 pixel density

    [Header("Colors")]
    public Color capeColor = new Color(0.53f, 0.07f, 0.21f);   // #881337 Crimson
    public Color suitColor = new Color(0.06f, 0.09f, 0.16f);   // #0f172a Midnight Dark
    public Color collarColor = new Color(0.74f, 0.07f, 0.23f); // #be123c Bright Red
    public Color skinColor = new Color(0.94f, 0.96f, 0.97f);   // #f1f5f9 Pale Marble
    public Color eyeColor = new Color(0.93f, 0.26f, 0.26f);    // #ef4444 Glowing Red

    [Header("Animation Settings")]
    public float waveSpeed = 8f;
    public float waveAmount = 1.5f; // Scaled down for 16x16 grid

    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    private Color32[] pixels;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // 1. Create a blank dynamic texture (16x16)
        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point; // Pixel crispness
        pixels = new Color32[textureWidth * textureHeight];

        // 2. Wrap into Unity Sprite (Pivot centered at feet)
        Sprite newSprite = Sprite.Create(
            texture,
            new Rect(0, 0, textureWidth, textureHeight),
            new Vector2(0.5f, 0f),
            pixelsPerUnit
        );
        spriteRenderer.sprite = newSprite;
    }

    void Update()
    {
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

        // Determine cape movement parameters
        float vx = rb != null ? Mathf.Abs(rb.linearVelocity.x) : 0f;
        float animationTime = Time.time * waveSpeed;
        float currentWave = Mathf.Sin(animationTime) * waveAmount * (1f + vx * 0.2f);

        // --- LAYER 1: Flowing Cape (Y: 1 to 10) ---
        int capeTopY = 10;
        int capeBottomY = 1;
        int baseLeftX = 4;
        int baseRightX = 11;

        for (int y = capeBottomY; y <= capeTopY; y++)
        {
            float progress = 1f - ((float)(y - capeBottomY) / (capeTopY - capeBottomY));
            int flare = Mathf.RoundToInt(progress * (2f + Mathf.Abs(currentWave) * 0.4f));
            int waveX = Mathf.RoundToInt(currentWave * progress);

            int startX = Mathf.Clamp(baseLeftX - flare + waveX, 0, textureWidth - 1);
            int endX = Mathf.Clamp(baseRightX + flare + waveX, 0, textureWidth - 1);

            for (int x = startX; x <= endX; x++)
            {
                SetPixel(x, y, capeColor);
            }
        }

        // --- LAYER 2: Vampiric Suit / Body (Y: 1 to 9, X: 5 to 10) ---
        for (int y = 1; y <= 9; y++)
        {
            for (int x = 5; x <= 10; x++)
            {
                SetPixel(x, y, suitColor);
            }
        }

        // --- LAYER 3: High Gothic Collar (Y: 8 to 11) ---
        // Left collar tips
        SetPixel(4, 11, collarColor); SetPixel(4, 10, collarColor);
        SetPixel(5, 9, collarColor); SetPixel(5, 8, collarColor);

        // Right collar tips
        SetPixel(11, 11, collarColor); SetPixel(11, 10, collarColor);
        SetPixel(10, 9, collarColor); SetPixel(10, 8, collarColor);

        // --- LAYER 4: Pale Face (Y: 9 to 13, X: 6 to 9) ---
        for (int y = 9; y <= 13; y++)
        {
            for (int x = 6; x <= 9; x++)
            {
                SetPixel(x, y, skinColor);
            }
        }

        // --- LAYER 5: Directional Glowing Red Eyes (Single Pixel each) ---
        int eyeXOffset = 0;
        if (rb != null)
        {
            if (rb.linearVelocity.x < -0.1f) eyeXOffset = -1;
            else if (rb.linearVelocity.x > 0.1f) eyeXOffset = 1;
        }

        // Left Eye (X=6) and Right Eye (X=8), shifted by movement
        SetPixel(6 + eyeXOffset, 11, eyeColor);
        SetPixel(8 + eyeXOffset, 11, eyeColor);

        // Apply array to Texture
        texture.SetPixels32(pixels);
        texture.Apply();
    }

    void SetPixel(int x, int y, Color color)
    {
        if (x >= 0 && x < textureWidth && y >= 0 && y < textureHeight)
        {
            pixels[y * textureWidth + x] = color;
        }
    }
}
