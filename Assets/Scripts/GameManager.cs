using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public enum GameState { Menu, Playing, LevelWin, LevelFailed }

    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState state = GameState.Playing;
    public bool IsPlaying => state == GameState.Playing;

    public static event Action<int> OnJumpCountChanged;
    public static event Action OnLevelWin;
    public static event Action<string> OnLevelFailed;

    private DraculaController dracula;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        dracula = FindObjectOfType<DraculaController>();
        // removed goalCrypt landing check
        //if (dracula != null)
        //    dracula.OnLanded += HandleLanding;
    }

    void HandleLanding(GameObject platformGO)
    {
        if (state != GameState.Playing) return;

        Platform platform = platformGO?.GetComponent<Platform>();
        if (platform == null || platform.type != Platform.PlatformType.GoalCrypt) return;

        bool keyRequired = platform.requiresKey;
        bool hasKey = dracula.HasKey;
        bool hasJumps = dracula.RemainingJumps > 0;

        if (keyRequired && !hasKey)
        {
            FailLevel("Locked Coffin! Find the Key");
            return;
        }

        if (hasJumps)
        {
            FailLevel("Extra energy is keeping Count D awake!");
            return;
        }

        WinLevel();
    }

    public void WinLevel()
    {
        state = GameState.LevelWin;
        //if (dracula != null)
        //{
        //    dracula.playerCanMove = false;
        //    dracula.StopMovement();
        //}
        MainMenuManager.UnlockNextLevel();
        Debug.Log("GameManager: Level Win!");
        OnLevelWin?.Invoke();
    }

    public void FailLevel(string reason)
    {
        state = GameState.LevelFailed;
        Debug.Log($"GameManager: Level Failed — {reason}");
        dracula.playerCanMove = false;
        dracula.StopMovement();
        OnLevelFailed?.Invoke(reason);
    }

    public void ReloadLevel()
    {
        Debug.Log("GameManager: Reloading level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadPreviousLevel()
    {
        int prev = SceneManager.GetActiveScene().buildIndex - 1;
        if (prev >= 0)
        {
            Debug.Log($"GameManager: Loading previous level (index {prev})");
            SceneManager.LoadScene(prev);
        }
        else
            Debug.Log("GameManager: Already at first level");
    }

    public void LoadNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"GameManager: Loading next level (index {next})");
            SceneManager.LoadScene(next);
        }
        else
        {
            Debug.Log("GameManager: No next level — looping to index 0");
            SceneManager.LoadScene(0);
        }
    }
}
