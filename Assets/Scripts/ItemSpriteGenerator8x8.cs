using UnityEngine;

public static class ItemSpriteGenerator8x8
{
    private static Color chaliceGold  = new Color(1f, 0.84f, 0f);
    private static Color bloodRed     = new Color(0.85f, 0.1f, 0.1f);
    private static Color sunYellow    = new Color(1f, 0.95f, 0.3f);
    private static Color batDark      = new Color(0.12f, 0.12f, 0.18f);
    private static Color keyGold      = new Color(0.95f, 0.75f, 0.15f);

    public static Sprite CreateItemSprite8x8(string itemType, float pixelsPerUnit = 8f)
    {
        int size = 8;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        Color32[] pixels = new Color32[size * size];

        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(0, 0, 0, 0);

        void SetPixel(int x, int y, Color c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                pixels[y * size + x] = c;
        }

        switch (itemType.ToLower())
        {
            case "chalice": // 8x8 Blood Chalice
                SetPixel(2, 0, chaliceGold); SetPixel(5, 0, chaliceGold); // Base
                SetPixel(3, 1, chaliceGold); SetPixel(4, 1, chaliceGold); // Stem
                // Cup
                for (int x = 2; x <= 5; x++) SetPixel(x, 2, chaliceGold);
                for (int x = 1; x <= 6; x++) SetPixel(x, 3, chaliceGold);
                for (int x = 1; x <= 6; x++) SetPixel(x, 4, bloodRed);     // Liquid
                SetPixel(1, 5, chaliceGold); SetPixel(6, 5, chaliceGold); // Rim
                break;

            case "sunstone": // 8x8 Sunstone
                // Sun Core
                for (int y = 2; y <= 5; y++)
                    for (int x = 2; x <= 5; x++) SetPixel(x, y, sunYellow);
                // Rays
                SetPixel(3, 7, sunYellow); SetPixel(4, 7, sunYellow); // Top
                SetPixel(3, 0, sunYellow); SetPixel(4, 0, sunYellow); // Bottom
                SetPixel(0, 3, sunYellow); SetPixel(0, 4, sunYellow); // Left
                SetPixel(7, 3, sunYellow); SetPixel(7, 4, sunYellow); // Right
                break;

            case "bat": // 8x8 Bat
                SetPixel(0, 5, batDark); SetPixel(7, 5, batDark); // Wingtips
                for (int x = 1; x <= 6; x++) SetPixel(x, 4, batDark);
                for (int x = 2; x <= 5; x++) SetPixel(x, 3, batDark);
                SetPixel(3, 2, batDark); SetPixel(4, 2, batDark); // Tail
                SetPixel(3, 4, bloodRed); SetPixel(4, 4, bloodRed); // Red Eyes
                break;

            case "key": // 8x8 Key
                // Bow Loop (Top)
                SetPixel(3, 7, keyGold); SetPixel(4, 7, keyGold);
                SetPixel(2, 6, keyGold); SetPixel(5, 6, keyGold);
                SetPixel(3, 5, keyGold); SetPixel(4, 5, keyGold);
                // Shaft & Teeth
                for (int y = 1; y <= 4; y++) SetPixel(3, y, keyGold);
                SetPixel(2, 2, keyGold); SetPixel(2, 1, keyGold); // Teeth
                break;
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }
}