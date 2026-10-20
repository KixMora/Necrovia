using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "SampleScene";

    [Header("Panels")]
    public GameObject mainButtonsGroup;
    public GameObject settingsPanel;

    [Header("Audio Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", 1f);

        if (!PlayerPrefs.HasKey("SFXVolume"))
            PlayerPrefs.SetFloat("SFXVolume", 1f);

        if (musicSlider != null)
        {
            float musicValue = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSlider.SetValueWithoutNotify(musicValue);

            if (MusicManager.Instance != null)
                MusicManager.Instance.SetVolume(musicValue);
        }

        if (sfxSlider != null)
        {
            float sfxValue = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.SetValueWithoutNotify(sfxValue);
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (mainButtonsGroup != null)
            mainButtonsGroup.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainButtonsGroup != null)
            mainButtonsGroup.SetActive(true);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        if (MusicManager.Instance != null)
            MusicManager.Instance.SetVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
}