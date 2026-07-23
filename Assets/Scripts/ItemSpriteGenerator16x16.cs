using UnityEngine;

public static class ItemSpriteGenerator16x16
{
    // Color Palette
    private static Color chaliceGold  = new Color(1f, 0.84f, 0f);      // Gold
    private static Color bloodRed     = new Color(0.85f, 0.1f, 0.1f);   // Crimson Blood
    private static Color sunYellow    = new Color(1f, 0.95f, 0.3f);    // Bright Sun
    private static Color sunOrange    = new Color(1f, 0.55f, 0f);      // Sun Rays
    private static Color batDark      = new Color(0.12f, 0.12f, 0.18f); // Midnight Bat Body
    private static Color keyGold      = new Color(0.95f, 0.75f, 0.15f); // Brass Gold
    private static Color outlineDark  = new Color(0.05f, 0.05f, 0.05f); // Deep Shadow Outline

    public static Sprite CreateItemSprite(string itemType, float pixelsPerUnit = 16f)
    {
        int size = 16;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        Color32[] pixels = new Color32[size * size];

        // Clear Background (Transparent)
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(0, 0, 0, 0);

        void SetPixel(int x, int y, Color c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                pixels[y * size + x] = c;
        }

        switch (itemType.ToLower())
        {
            case "chalice": // --- BLOOD CHALICE (+1 Jump) ---
                // Base / Stem
                for (int x = 5; x <= 10; x++) SetPixel(x, 1, outlineDark);
                for (int x = 6; x <= 9; x++)  SetPixel(x, 2, chaliceGold);
                for (int y = 3; y <= 5; y++)  { SetPixel(7, y, chaliceGold); SetPixel(8, y, chaliceGold); }
                // Cup Outer & Gold
                for (int y = 6; y <= 13; y++)
                {
                    int width = y <= 10 ? (y - 5) + 3 : 8;
                    int startX = 8 - (width / 2);
                    int endX = startX + width - 1;
                    for (int x = startX; x <= endX; x++)
                    {
                        if (x == startX || x == endX || y == 6) SetPixel(x, y, outlineDark);
                        else SetPixel(x, y, chaliceGold);
                    }
                }
                // Blood Liquid Pool
                for (int y = 10; y <= 12; y++)
                    for (int x = 5; x <= 10; x++) SetPixel(x, y, bloodRed);
                break;

            case "sunstone": // --- SUNSTONE (-1 Jump) ---
                // Core Sun Circle
                for (int y = 4; y <= 11; y++)
                {
                    for (int x = 4; x <= 11; x++)
                    {
                        float dist = Vector2.Distance(new Vector2(x, y), new Vector2(7.5f, 7.5f));
                        if (dist <= 3.8f) SetPixel(x, y, sunYellow);
                        else if (dist <= 4.5f) SetPixel(x, y, sunOrange);
                    }
                }
                // 8 Sun Rays
                SetPixel(7, 14, sunOrange); SetPixel(8, 14, sunOrange); // Top
                SetPixel(7, 1, sunOrange);  SetPixel(8, 1, sunOrange);  // Bottom
                SetPixel(1, 7, sunOrange);  SetPixel(1, 8, sunOrange);  // Left
                SetPixel(14, 7, sunOrange); SetPixel(14, 8, sunOrange); // Right
                SetPixel(2, 13, sunYellow); SetPixel(13, 13, sunYellow); // Diagonals
                SetPixel(2, 2, sunYellow);  SetPixel(13, 2, sunYellow);
                break;

            case "bat": // --- BAT SWARM (*2 Jumps) ---
                // Big Bat Center
                for (int x = 4; x <= 11; x++) SetPixel(x, 9, batDark);
                for (int x = 2; x <= 13; x++) SetPixel(x, 10, batDark);
                SetPixel(1, 11, batDark); SetPixel(14, 11, batDark); // Wingtips
                SetPixel(7, 8, batDark);  SetPixel(8, 8, batDark);    // Body
                SetPixel(7, 11, bloodRed); SetPixel(8, 11, bloodRed); // Eyes
                // Little Bat Top Right
                SetPixel(12, 14, batDark); SetPixel(14, 14, batDark);
                SetPixel(11, 13, batDark); SetPixel(15, 13, batDark);
                // Little Bat Bottom Left
                SetPixel(2, 4, batDark); SetPixel(4, 4, batDark);
                SetPixel(1, 3, batDark); SetPixel(5, 3, batDark);
                break;

            case "key": // --- GOTHIC KEY ---
                // Bow / Handle Loop (Top)
                for (int y = 10; y <= 14; y++)
                {
                    for (int x = 5; x <= 10; x++)
                    {
                        if (x == 5 || x == 10 || y == 10 || y == 14) SetPixel(x, y, keyGold);
                    }
                }
                // Shaft
                for (int y = 2; y <= 9; y++)
                {
                    SetPixel(7, y, keyGold);
                    SetPixel(8, y, keyGold);
                }
                // Bit / Teeth (Bottom Left & Right)
                SetPixel(5, 3, keyGold); SetPixel(6, 3, keyGold);
                SetPixel(5, 5, keyGold); SetPixel(6, 5, keyGold);
                break;
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }
}