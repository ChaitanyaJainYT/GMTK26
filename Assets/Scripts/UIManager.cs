using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TMP_Text levelTitleText;
    [SerializeField] private TMP_Text jumpCountText;
    [SerializeField] private Image keyIcon;

    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private TMP_Text failReasonText;

    [Header("Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextFloorButton;

    [Header("Level Info")]
    [SerializeField] private string levelTitle = "Crypt 1";

    private DraculaController dracula;

    void Awake()
    {
        winPanel?.SetActive(false);
        failPanel?.SetActive(false);
    }

    void Start()
    {
        dracula = FindObjectOfType<DraculaController>();

        if (jumpCountText == null)
            jumpCountText = dracula?.GetComponentInChildren<TMP_Text>();

        if (levelTitleText != null)
            levelTitleText.text = levelTitle;

        if (keyIcon != null)
            keyIcon.enabled = false;

        UpdateJumpDisplay(dracula != null ? dracula.RemainingJumps : 0);

        GameManager.OnLevelWin += ShowWinPanel;
        GameManager.OnLevelFailed += ShowFailPanel;

        if (retryButton != null)
            retryButton.onClick.AddListener(() => GameManager.Instance?.ReloadLevel());

        if (nextFloorButton != null)
            nextFloorButton.onClick.AddListener(() => GameManager.Instance?.LoadNextLevel());
    }

    void Update()
    {
        if (dracula == null) return;

        UpdateJumpDisplay(dracula.RemainingJumps);

        if (keyIcon != null)
            keyIcon.enabled = dracula.HasKey;

        if (Input.GetKeyDown(KeyCode.R))
            GameManager.Instance?.ReloadLevel();

        bool confirm = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
        if (confirm)
        {
            if (winPanel != null && winPanel.activeSelf && nextFloorButton != null)
                nextFloorButton.onClick.Invoke();
            else if (failPanel != null && failPanel.activeSelf && retryButton != null)
                retryButton.onClick.Invoke();
        }
    }

    private void UpdateJumpDisplay(int count)
    {
        if (jumpCountText != null)
            jumpCountText.text = $"{count}";
    }

    private void ShowWinPanel()
    {
        winPanel?.SetActive(true);
    }

    private void ShowFailPanel(string reason)
    {
        if (failReasonText != null)
            failReasonText.text = reason;
        failPanel?.SetActive(true);
    }

    void OnDestroy()
    {
        GameManager.OnLevelWin -= ShowWinPanel;
        GameManager.OnLevelFailed -= ShowFailPanel;
    }
}
