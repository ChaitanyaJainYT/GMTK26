using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class LevelArrangerWindow : EditorWindow
{
    private ReorderableList reorderableList;
    private List<EditorBuildSettingsScene> buildScenes;
    private Dictionary<string, Texture2D> scenePreviews = new Dictionary<string, Texture2D>();
    private Vector2 scrollPosition;

    private const string PREVIEW_FOLDER = "Assets/Editor/LevelPreviews";

    [MenuItem("Tools/Level Arranger")]
    public static void ShowWindow()
    {
        GetWindow<LevelArrangerWindow>("Level Arranger");
    }

    private void OnEnable()
    {
        LoadBuildSettings();
        LoadExistingPreviews();
        SetupReorderableList();
    }

    private void LoadBuildSettings()
    {
        buildScenes = EditorBuildSettings.scenes.ToList();
    }

    private void SetupReorderableList()
    {
        reorderableList = new ReorderableList(buildScenes, typeof(EditorBuildSettingsScene), true, true, false, true);

        reorderableList.elementHeight = 80;

        reorderableList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Drag to Reorder Levels (Syncs to Build Settings)");
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            if (index < 0 || index >= buildScenes.Count) return;

            var scene = buildScenes[index];
            string sceneName = Path.GetFileNameWithoutExtension(scene.path);

            // Thumbnail Rect
            Rect texRect = new Rect(rect.x, rect.y + 2, 130, 76);
            if (scenePreviews.TryGetValue(scene.path, out Texture2D tex) && tex != null)
            {
                GUI.DrawTexture(texRect, tex, ScaleMode.ScaleToFit);
            }
            else
            {
                EditorGUI.HelpBox(texRect, "No Preview", MessageType.None);
            }

            // Name and Status Rect
            Rect labelRect = new Rect(rect.x + 140, rect.y + 30, rect.width - 150, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, $"{index}. {sceneName}", EditorStyles.boldLabel);

            // Enable/Disable toggle
            Rect toggleRect = new Rect(rect.x + 140, rect.y + 50, 100, EditorGUIUtility.singleLineHeight);

            // FIX: Listen specifically for this checkbox being clicked
            EditorGUI.BeginChangeCheck();
            bool isEnabled = EditorGUI.ToggleLeft(toggleRect, "Enabled", scene.enabled);
            if (EditorGUI.EndChangeCheck())
            {
                scene.enabled = isEnabled;
                SaveToBuildSettings(); // Push the change to Unity immediately
            }
        };

        reorderableList.onChangedCallback = (ReorderableList list) => {
            SaveToBuildSettings();
        };

        reorderableList.onRemoveCallback = (ReorderableList list) => {
            buildScenes.RemoveAt(list.index);
            SaveToBuildSettings();
            GUIUtility.ExitGUI();
        };
    }

    private void SaveToBuildSettings()
    {
        EditorBuildSettings.scenes = buildScenes.ToArray();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        EditorGUILayout.HelpBox("Drag scene assets from the Project window anywhere into this window to add them.", MessageType.Info);

        // Top Button Row
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh List", GUILayout.Height(25)))
        {
            RefreshData();
        }
        if (GUILayout.Button("Add All Scenes In Project", GUILayout.Height(25)))
        {
            AddAllScenesInProject();
        }
        if (GUILayout.Button("Generate Camera Previews (Takes a moment)", GUILayout.Height(25)))
        {
            GenerateThumbnails();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        reorderableList.DoLayoutList();
        EditorGUILayout.EndScrollView();

        // Handle Drag and Drop events over the window
        HandleDragAndDrop();
    }

    private void RefreshData()
    {
        LoadBuildSettings();
        LoadExistingPreviews();
        Repaint();
    }

    private void AddAllScenesInProject()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene");
        bool changed = false;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Check if it's already in the list to avoid duplicates
            if (!buildScenes.Any(s => s.path == path))
            {
                buildScenes.Add(new EditorBuildSettingsScene(path, true));
                changed = true;
            }
        }

        if (changed)
        {
            SaveToBuildSettings();
            LoadExistingPreviews();
        }
    }

    private void HandleDragAndDrop()
    {
        Event evt = Event.current;
        Rect dropArea = new Rect(0, 0, position.width, position.height);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    bool changed = false;

                    foreach (string path in DragAndDrop.paths)
                    {
                        // Ensure we are only dropping Unity scenes
                        if (path.EndsWith(".unity"))
                        {
                            if (!buildScenes.Any(s => s.path == path))
                            {
                                buildScenes.Add(new EditorBuildSettingsScene(path, true));
                                changed = true;
                            }
                        }
                    }

                    if (changed)
                    {
                        SaveToBuildSettings();
                        LoadExistingPreviews();
                    }
                }
                break;
        }
    }

    private void GenerateThumbnails()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        if (!Directory.Exists(PREVIEW_FOLDER))
        {
            Directory.CreateDirectory(PREVIEW_FOLDER);
        }

        string startingScenePath = EditorSceneManager.GetActiveScene().path;

        try
        {
            for (int i = 0; i < buildScenes.Count; i++)
            {
                var sceneInfo = buildScenes[i];
                if (string.IsNullOrEmpty(sceneInfo.path)) continue;

                EditorUtility.DisplayProgressBar("Generating Previews", $"Opening {Path.GetFileNameWithoutExtension(sceneInfo.path)}", (float)i / buildScenes.Count);

                var scene = EditorSceneManager.OpenScene(sceneInfo.path, OpenSceneMode.Single);

                Camera cam = Camera.main;
                if (cam == null) cam = FindObjectOfType<Camera>();

                if (cam != null)
                {
                    Texture2D tex = CaptureCameraOutput(cam);
                    SaveTextureAsPNG(tex, sceneInfo.path);
                    DestroyImmediate(tex);
                }
                else
                {
                    Debug.LogWarning($"Level Arranger: No camera found in scene '{scene.name}'. Skipping preview.");
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            if (!string.IsNullOrEmpty(startingScenePath))
            {
                EditorSceneManager.OpenScene(startingScenePath);
            }
            LoadExistingPreviews();
        }
    }

    private Texture2D CaptureCameraOutput(Camera camera)
    {
        int width = 256;
        int height = 144;

        RenderTexture rt = new RenderTexture(width, height, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);

        camera.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        return screenShot;
    }

    private void SaveTextureAsPNG(Texture2D tex, string scenePath)
    {
        byte[] bytes = tex.EncodeToPNG();
        string safeName = scenePath.Replace("/", "_").Replace(".unity", ".png");
        string filePath = Path.Combine(PREVIEW_FOLDER, safeName);
        File.WriteAllBytes(filePath, bytes);
    }

    private void LoadExistingPreviews()
    {
        scenePreviews.Clear();
        if (!Directory.Exists(PREVIEW_FOLDER)) return;

        foreach (var scene in buildScenes)
        {
            string safeName = scene.path.Replace("/", "_").Replace(".unity", ".png");
            string filePath = Path.Combine(PREVIEW_FOLDER, safeName);

            if (File.Exists(filePath))
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                scenePreviews[scene.path] = tex;
            }
        }
    }
}