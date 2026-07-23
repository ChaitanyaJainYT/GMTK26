using UnityEngine;
using System.IO;

public static class TextureSaver
{
    private static string UniquePath(string basePath)
    {
        string dir = Path.GetDirectoryName(basePath);
        string name = Path.GetFileNameWithoutExtension(basePath);
        string ext = Path.GetExtension(basePath);

        string path = basePath;
        int suffix = 1;
        while (File.Exists(path))
            path = Path.Combine(dir, $"{name}_{suffix++}{ext}");
        return path;
    }

    public static void SaveTexture(Texture2D texture, string filename)
    {
        if (texture == null)
        {
            Debug.LogWarning("TextureSaver: Texture is null.");
            return;
        }

        byte[] bytes = texture.EncodeToPNG();
        string path = UniquePath(Path.Combine(Application.dataPath, $"{filename}.png"));
        File.WriteAllBytes(path, bytes);
        Debug.Log($"TextureSaver: Saved {path}");
    }

    public static void SaveDraculaFrame(string filename)
    {
        ProceduralDracula2D dracula = Object.FindObjectOfType<ProceduralDracula2D>();
        if (dracula == null)
        {
            Debug.LogWarning("TextureSaver: No ProceduralDracula2D found.");
            return;
        }

        byte[] bytes = dracula.EncodeCurrentFrameToPNG();
        string path = UniquePath(Path.Combine(Application.dataPath, $"{filename}.png"));
        File.WriteAllBytes(path, bytes);
        Debug.Log($"TextureSaver: Saved {path}");
    }

    public static void SaveItemTesterFrame(string filename)
    {
        ItemSpriteTester tester = Object.FindObjectOfType<ItemSpriteTester>();
        if (tester == null)
        {
            Debug.LogWarning("TextureSaver: No ItemSpriteTester found.");
            return;
        }

        SpriteRenderer sr = tester.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
        {
            Debug.LogWarning("TextureSaver: ItemSpriteTester has no sprite.");
            return;
        }

        SaveTexture(sr.sprite.texture, filename);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Save Dracula Texture")]
    private static void SaveDraculaFromEditor()
    {
        SaveDraculaFrame("DraculaSprite");
    }

    [UnityEditor.MenuItem("Tools/Save Item Tester Texture")]
    private static void SaveItemFromEditor()
    {
        SaveItemTesterFrame("ItemSprite");
    }
#endif
}
