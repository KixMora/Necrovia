using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class MissionResultUI : MonoBehaviour
{
    public static MissionResultUI Instance { get; private set; }
    private const string LanguageKey = "selected_language";

    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button retryButton;
    // [SerializeField] private Button backToMapButton;
    [SerializeField] private Button mainMenuButton;

    // [Header("Button Texts")]
    // [SerializeField] private TMP_Text continueButtonText;
    // [SerializeField] private TMP_Text retryButtonText;
    // [SerializeField] private TMP_Text backToMapButtonText;
    // [SerializeField] private TMP_Text mainMenuButtonText;

    [Header("Scene Names")]
    // [SerializeField] private string missionMapSceneName = "MissionSelect";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private MissionEndType currentType;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (root != null)
            root.SetActive(false);
    }

    public void ShowCompleted()
    {
        Show(MissionEndType.Completed);
    }

    public void ShowFailed()
    {
        Show(MissionEndType.Failed);
    }

    public void ShowExitToMenu()
    {
        Show(MissionEndType.ExitToMenu);
    }

    public void Show(MissionEndType type)
    {
        currentType = type;

        if (root != null)
            root.SetActive(true);

        Time.timeScale = 0f;

        SetupTexts(type);
        SetupButtons(type);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);

        Time.timeScale = 1f;
    }
    private bool IsRussian()
    {
        if (LocalizationSettings.SelectedLocale != null)
            return LocalizationSettings.SelectedLocale.Identifier.Code == "ru";

        string lang = PlayerPrefs.GetString(LanguageKey, "ru");
        return lang == "ru";
    }
    private void SetupTexts(MissionEndType type)
    {
        bool ru = IsRussian();

        switch (type)
        {
            case MissionEndType.Completed:
                if (ru)
                {
                    SetTexts(
                        "МИССИЯ ВЫПОЛНЕНА",
                        "Вы успешно завершили задание."
                    );
                }
                else
                {
                    SetTexts(
                        "MISSION COMPLETED",
                        "You have successfully completed the mission."
                    );
                }
                break;

            case MissionEndType.Failed:
                if (ru)
                {
                    SetTexts(
                        "МИССИЯ ПРОВАЛЕНА",
                        "Вы погибли. Попробуйте снова."
                    );
                }
                else
                {
                    SetTexts(
                        "MISSION FAILED",
                        "You died. Try again."
                    );
                }
                break;

            case MissionEndType.ExitToMenu:
                if (ru)
                {
                    SetTexts(
                        "ВЫЙТИ ИЗ МИССИИ?",
                        "Прогресс текущей миссии будет потерян."
                    );
                }
                else
                {
                    SetTexts(
                        "EXIT MISSION?",
                        "Current mission progress will be lost."
                    );
                }
                break;
        }
    }

    private void SetTexts(string title, string description)
    {
        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;
    }

    private void SetupButtons(MissionEndType type)
    {
        SetButtonActive(continueButton, false);
        SetButtonActive(retryButton, false);
        // SetButtonActive(backToMapButton, false);
        SetButtonActive(mainMenuButton, false);

        switch (type)
        {
            case MissionEndType.Completed:
                // SetButtonActive(backToMapButton, true);
                SetButtonActive(mainMenuButton, true);
                break;

            case MissionEndType.Failed:
                SetButtonActive(retryButton, true);
                // SetButtonActive(backToMapButton, true);
                SetButtonActive(mainMenuButton, true);
                break;

            case MissionEndType.ExitToMenu:
                SetButtonActive(continueButton, true);
                SetButtonActive(mainMenuButton, true);
                break;
        }

        // if (continueButtonText != null)
        //     continueButtonText.text = "ПРОДОЛЖИТЬ";

        // if (retryButtonText != null)
        //     retryButtonText.text = "ПОВТОРИТЬ";

        // if (backToMapButtonText != null)
        //     backToMapButtonText.text = "КАРТА МИССИЙ";

        // if (mainMenuButtonText != null)
        //     mainMenuButtonText.text = "ГЛАВНОЕ МЕНЮ";
    }

    private void SetButtonActive(Button button, bool active)
    {
        if (button != null)
            button.gameObject.SetActive(active);
    }

    public void ContinueGame()
    {
        Hide();
    }

    public void RetryMission()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // public void BackToMap()
    // {
    //     Time.timeScale = 1f;

    //     if (SceneTransitionManager.Instance != null)
    //     {
    //         SceneTransitionManager.Instance.LoadScene(missionMapSceneName);
    //     }
    //     else
    //     {
    //         SceneManager.LoadScene(missionMapSceneName);
    //     }
    // }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(mainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}