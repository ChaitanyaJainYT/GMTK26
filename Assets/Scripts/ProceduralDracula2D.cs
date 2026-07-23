using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProceduralDracula2D : MonoBehaviour
{
    [Header("Sprite Canvas Dimensions")]
    public int textureWidth = 16;
    public int textureHeight = 16;
    public float pixelsPerUnit = 16f;

    [Header("Colors")]
    public Color capeColor = new Color(0.42f, 0.06f, 0.17f);   // #6C0E2B Crimson
    public Color suitColor = new Color(0.06f, 0.09f, 0.16f);   // #0f172a Midnight Dark
    public Color collarColor = new Color(0.74f, 0.07f, 0.23f); // #be123c Bright Red
    public Color skinColor = new Color(0.94f, 0.96f, 0.97f);   // #f1f5f9 Pale Marble
    public Color eyeColor = new Color(0.93f, 0.26f, 0.26f);    // #ef4444 Glowing Red

    [Header("Animation Settings")]
    public float waveSpeed = 10f;
    public float waveAmount = 1.2f;
    public float dragStrength = 0.8f;

    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    private Color32[] pixels;
    private Rigidbody2D rb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        pixels = new Color32[textureWidth * textureHeight];

        Sprite newSprite = Sprite.Create(
            texture, 
            new Rect(0, 0, textureWidth, textureHeight), 
            new Vector2(0.5f, 0f), // Pivot at bottom center
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
        // Clear background
        Color32 clearColor = new Color32(0, 0, 0, 0);
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clearColor;
        }

        // Horizontal velocity drag
        float vx = rb != null ? rb.linearVelocity.x : 0f;
        float movementDrag = -vx * dragStrength;

        // Wave oscillation
        float animationTime = Time.time * waveSpeed;
        float currentWave = Mathf.Sin(animationTime) * waveAmount;

        // --- LAYER 1: Flowing Cape (Full Y: 0 to 12, Edge X bounds) ---
        int capeTopY = 12;
        int capeBottomY = 0; // Starts directly at Y=0 (no bottom gap)
        int baseLeftX = 3;
        int baseRightX = 12;

        for (int y = capeBottomY; y <= capeTopY; y++)
        {
            float progress = 1f - ((float)(y - capeBottomY) / (capeTopY - capeBottomY));
            int flare = Mathf.RoundToInt(progress * (3f + Mathf.Abs(currentWave) * 0.4f));
            int totalShiftX = Mathf.RoundToInt((movementDrag + currentWave) * progress);

            // Spans fully from X=0 to X=15 at max flare
            int startX = Mathf.Clamp(baseLeftX - flare + totalShiftX, 0, textureWidth - 1);
            int endX = Mathf.Clamp(baseRightX + flare + totalShiftX, 0, textureWidth - 1);

            for (int x = startX; x <= endX; x++)
            {
                SetPixel(x, y, capeColor);
            }
        }

        // --- LAYER 2: Suit Body (Full Y: 0 to 11, X: 4 to 11) ---
        for (int y = 0; y <= 11; y++)
        {
            for (int x = 4; x <= 11; x++)
            {
                SetPixel(x, y, suitColor);
            }
        }

        // --- LAYER 3: Collar Tips (Y: 9 to 13) ---
        SetPixel(3, 13, collarColor); SetPixel(3, 12, collarColor);
        SetPixel(4, 11, collarColor); SetPixel(4, 10, collarColor);
        
        SetPixel(12, 13, collarColor); SetPixel(12, 12, collarColor);
        SetPixel(11, 11, collarColor); SetPixel(11, 10, collarColor);

        // --- LAYER 4: Face (Spans up to top Y=15, X: 5 to 10) ---
        for (int y = 10; y <= 15; y++) // Fills up to top edge Y=15
        {
            for (int x = 5; x <= 10; x++)
            {
                SetPixel(x, y, skinColor);
            }
        }

        // --- LAYER 5: Eyes ---
        int eyeXOffset = 0;
        if (vx < -0.1f) eyeXOffset = -1;
        else if (vx > 0.1f) eyeXOffset = 1;

        SetPixel(6 + eyeXOffset, 12, eyeColor);
        SetPixel(9 + eyeXOffset, 12, eyeColor);

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
