using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class WaveUI : MonoBehaviour
{
    [Header("References")]
    public WaveManager waveManager;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text statusText;

    private const string LanguageKey = "selected_language";

    private void Update()
    {
        if (waveManager == null)
            return;

        UpdateWaveText();
        UpdateStatusText();
    }

    private void UpdateWaveText()
    {
        if (waveText == null)
            return;

        int shownWave = Mathf.Max(1, waveManager.CurrentWave);

        if (IsRussian())
        {
            waveText.text = "Волна " + shownWave;
        }
        else
        {
            waveText.text = "Wave " + shownWave;
        }
    }

    private void UpdateStatusText()
    {
        if (statusText == null)
            return;

        if (waveManager.WaveInProgress)
        {
            if (IsRussian())
            {
                statusText.text = "Осталось: " + waveManager.EnemiesAliveInWave;
            }
            else
            {
                statusText.text = "Remaining: " + waveManager.EnemiesAliveInWave;
            }
        }
        else if (waveManager.WaitingForNextWave)
        {
            int seconds = Mathf.CeilToInt(waveManager.NextWaveCountdown);

            if (IsRussian())
            {
                statusText.text = "Новая волна через: " + seconds;
            }
            else
            {
                statusText.text = "New wave in: " + seconds;
            }
        }
        else
        {
            statusText.text = "";
        }
    }

    private bool IsRussian()
    {
        // Сначала пробуем взять язык из Unity Localization
        if (LocalizationSettings.SelectedLocale != null)
        {
            return LocalizationSettings.SelectedLocale.Identifier.Code == "ru";
        }

        // Если Localization ещё не успел инициализироваться — берём из PlayerPrefs
        string lang = PlayerPrefs.GetString(LanguageKey, "ru");
        return lang == "ru";
    }
}