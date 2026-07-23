using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ProceduralDracula2D : MonoBehaviour
{
    [Header("Sprite Canvas Dimensions")]
    public int textureWidth = 16;
    public int textureHeight = 16;
    public float pixelsPerUnit = 16f;

    [Header("Colors")]
    public Color capeColor = new Color(0.53f, 0.07f, 0.21f);   // #881337 Crimson
    public Color suitColor = new Color(0.06f, 0.09f, 0.16f);   // #0f172a Midnight Dark
    public Color collarColor = new Color(0.74f, 0.07f, 0.23f); // #be123c Bright Red
    public Color skinColor = new Color(0.94f, 0.96f, 0.97f);   // #f1f5f9 Pale Marble
    public Color eyeColor = new Color(0.93f, 0.26f, 0.26f);    // #ef4444 Glowing Red

    [Header("Animation Settings")]
    public float waveSpeed = 10f;
    public float waveAmount = 1.2f;
    public float dragStrength = 0.8f; // How strongly the cape trails behind velocity

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
        // Clear background
        Color32 clearColor = new Color32(0, 0, 0, 0);
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = clearColor;
        }

        // Get horizontal velocity
        float vx = rb != null ? rb.linearVelocity.x : 0f;

        // 1. Calculate OPPOSE DRAG (Dracula moves Right [vx > 0] -> Cape trails Left [-offset])
        float movementDrag = -vx * dragStrength;

        // 2. Dynamic idle/movement wave
        float animationTime = Time.time * waveSpeed;
        float currentWave = Mathf.Sin(animationTime) * waveAmount;

        // --- LAYER 1: Flowing Opposing Cape (Y: 1 to 10) ---
        int capeTopY = 10;
        int capeBottomY = 1;
        int baseLeftX = 4;
        int baseRightX = 11;

        for (int y = capeBottomY; y <= capeTopY; y++)
        {
            // Progress goes from 0.0 (top anchored at shoulders) to 1.0 (bottom flowing free)
            float progress = 1f - ((float)(y - capeBottomY) / (capeTopY - capeBottomY));

            // Flare outwards at bottom + wave oscillation
            int flare = Mathf.RoundToInt(progress * (2f + Mathf.Abs(currentWave) * 0.3f));

            // Shift cape OPPOSITE to movement direction (scaled by progress down the cape)
            int totalShiftX = Mathf.RoundToInt((movementDrag + currentWave) * progress);

            int startX = Mathf.Clamp(baseLeftX - flare + totalShiftX, 0, textureWidth - 1);
            int endX = Mathf.Clamp(baseRightX + flare + totalShiftX, 0, textureWidth - 1);

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
        SetPixel(4, 11, collarColor); SetPixel(4, 10, collarColor);
        SetPixel(5, 9, collarColor); SetPixel(5, 8, collarColor);

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

        // --- LAYER 5: Directional Glowing Red Eyes ---
        int eyeXOffset = 0;
        if (vx < -0.1f) eyeXOffset = -1;
        else if (vx > 0.1f) eyeXOffset = 1;

        SetPixel(6 + eyeXOffset, 11, eyeColor);
        SetPixel(8 + eyeXOffset, 11, eyeColor);

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
