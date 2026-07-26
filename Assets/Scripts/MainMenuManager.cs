using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Main Panel")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Level Select")]
    [SerializeField] private Transform levelButtonContainer;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private ScrollRect levelScrollRect;
    [SerializeField] private Button levelBackButton;

    [Header("Settings")]
    [SerializeField] private Button bgmButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Image bgmImage;
    [SerializeField] private Image sfxImage;
    [SerializeField] private Sprite bgmOnSprite;
    [SerializeField] private Sprite bgmOffSprite;
    [SerializeField] private Sprite sfxOnSprite;
    [SerializeField] private Sprite sfxOffSprite;
    [SerializeField] private Button settingsBackButton;

    private bool bgmOn;
    private bool sfxOn;

    void Start()
    {
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        settingsPanel.SetActive(false);

        playButton.onClick.AddListener(ShowLevelSelect);
        settingsButton.onClick.AddListener(ShowSettings);
        quitButton.onClick.AddListener(() => Application.Quit());
        levelBackButton.onClick.AddListener(ShowMain);
        settingsBackButton.onClick.AddListener(ShowMain);

        bgmOn = PlayerPrefs.GetInt("BGMEnabled", 1) == 1;
        sfxOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        UpdateSettingVisuals();

        bgmButton.onClick.AddListener(ToggleBGM);
        sfxButton.onClick.AddListener(ToggleSFX);

        PopulateLevelButtons();
    }

    private void ToggleBGM()
    {
        bgmOn = !bgmOn;
        PlayerPrefs.SetInt("BGMEnabled", bgmOn ? 1 : 0);
        UpdateSettingVisuals();
        if (AudioManager.Instance != null)
            AudioManager.Instance.ApplySettings();
    }

    private void ToggleSFX()
    {
        sfxOn = !sfxOn;
        PlayerPrefs.SetInt("SFXEnabled", sfxOn ? 1 : 0);
        UpdateSettingVisuals();
        if (AudioManager.Instance != null)
            AudioManager.Instance.ApplySettings();
    }

    private void UpdateSettingVisuals()
    {
        if (bgmImage != null)
            bgmImage.sprite = bgmOn ? bgmOnSprite : bgmOffSprite;

        if (sfxImage != null)
            sfxImage.sprite = sfxOn ? sfxOnSprite : sfxOffSprite;
    }

    private void ShowMain()
    {
        mainPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    private void ShowLevelSelect()
    {
        mainPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    private void ShowSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void PopulateLevelButtons()
    {
        for (int i = 0; i < levelButtonContainer.childCount; i++)
            Destroy(levelButtonContainer.GetChild(i).gameObject);

        if (levelScrollRect != null)
            levelScrollRect.movementType = ScrollRect.MovementType.Clamped;

        ContentSizeFitter fitter = levelButtonContainer.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = levelButtonContainer.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 1; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            GameObject btnObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = name;

            int index = i;
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => SceneManager.LoadScene(index));
        }
    }
}
