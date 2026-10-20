using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Menu Scenes")]
    public List<string> menuSceneNames = new List<string>();

    [Header("Game Scenes")]
    public List<string> gameSceneNames = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", 1f);

        ApplySavedVolume();
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySavedVolume();
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        if (menuSceneNames.Contains(sceneName))
        {
            PlayMusic(menuMusic);
        }
        else if (gameSceneNames.Contains(sceneName))
        {
            PlayMusic(gameMusic);
        }
        else
        {
            Debug.Log("[MusicManager] No music rule for scene: " + sceneName);
        }
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayGameMusic()
    {
        PlayMusic(gameMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    public void SetVolume(float value)
    {
        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", 1f);

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        if (audioSource != null)
        {
            audioSource.volume = value;

            if (audioSource.clip != null && !audioSource.isPlaying && value > 0.001f)
                audioSource.Play();
        }
    }

    public void ApplySavedVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (audioSource != null)
            audioSource.volume = savedVolume;
    }

    public float GetSavedVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 1f);
    }
}