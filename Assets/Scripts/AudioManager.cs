using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip bgmClip;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplySettings();
    }

    void Start()
    {
        if (bgmClip != null && bgmSource.clip == null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySettings();
    }

    public void ApplySettings()
    {
        bool bgmOn = PlayerPrefs.GetInt("BGMEnabled", 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        if (bgmSource != null)
            bgmSource.mute = !bgmOn;

        AudioSource[] allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource src in allSources)
        {
            if (src == bgmSource) continue;
            src.mute = !sfxOn;
        }
    }
}
