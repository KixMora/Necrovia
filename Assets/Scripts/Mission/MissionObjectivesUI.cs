using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MissionObjectivesUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private MissionObjectiveLineUI linePrefab;

    private readonly List<MissionObjectiveLineUI> activeLines = new List<MissionObjectiveLineUI>();
    private MissionData currentMission;

    private const string LanguageKey = "selected_language";

    private void Start()
    {
        if (MissionManager.Instance != null)
            currentMission = MissionManager.Instance.SelectedMission;

        BuildUI();
    }

    private void Update()
    {
        RefreshUI();
    }

    private void BuildUI()
    {
        if (contentRoot == null || linePrefab == null)
        {
            Debug.LogError("[MissionObjectivesUI] contentRoot or linePrefab is NULL");
            return;
        }

        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        activeLines.Clear();

        if (currentMission == null || currentMission.objectives == null)
            return;

        for (int i = 0; i < currentMission.objectives.Count; i++)
        {
            MissionObjectiveLineUI line = Instantiate(linePrefab, contentRoot);
            activeLines.Add(line);
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (currentMission == null || currentMission.objectives == null)
            return;

        if (MissionRuntimeTracker.Instance == null)
            return;

        for (int i = 0; i < currentMission.objectives.Count; i++)
        {
            if (i >= activeLines.Count)
                break;

            MissionObjective objective = currentMission.objectives[i];
            MissionObjectiveLineUI line = activeLines[i];

            bool completed = IsObjectiveCompleted(objective);
            string text = BuildObjectiveText(objective);

            line.SetText(text, completed);
        }
    }

    private string BuildObjectiveText(MissionObjective objective)
    {
        bool ru = IsRussian();

        switch (objective.type)
        {
            case MissionObjectiveType.KillZombies:
            {
                int currentValue = MissionRuntimeTracker.Instance.ZombiesKilled;

                if (ru)
                    return $"Уничтожить зомби: {currentValue} / {objective.targetValue}";

                return $"Kill zombies: {currentValue} / {objective.targetValue}";
            }

            case MissionObjectiveType.SurviveWaves:
            {
                int currentValue = MissionRuntimeTracker.Instance.WavesSurvived;

                if (ru)
                    return $"Пережить волн: {currentValue} / {objective.targetValue}";

                return $"Survive waves: {currentValue} / {objective.targetValue}";
            }

            case MissionObjectiveType.SurviveTime:
            {
                int currentValue = Mathf.FloorToInt(MissionRuntimeTracker.Instance.TimeSurvived);

                if (ru)
                    return $"Выжить: {FormatTime(currentValue)} / {FormatTime(objective.targetValue)}";

                return $"Survive time: {FormatTime(currentValue)} / {FormatTime(objective.targetValue)}";
            }

            default:
            {
                if (ru)
                    return "Неизвестная цель";

                return "Unknown objective";
            }
        }
    }

    private bool IsObjectiveCompleted(MissionObjective objective)
    {
        switch (objective.type)
        {
            case MissionObjectiveType.KillZombies:
                return MissionRuntimeTracker.Instance.ZombiesKilled >= objective.targetValue;

            case MissionObjectiveType.SurviveWaves:
                return MissionRuntimeTracker.Instance.WavesSurvived >= objective.targetValue;

            case MissionObjectiveType.SurviveTime:
                return MissionRuntimeTracker.Instance.TimeSurvived >= objective.targetValue;

            default:
                return false;
        }
    }

    private string FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    private bool IsRussian()
    {
        if (LocalizationSettings.SelectedLocale != null)
            return LocalizationSettings.SelectedLocale.Identifier.Code == "ru";

        string lang = PlayerPrefs.GetString(LanguageKey, "ru");
        return lang == "ru";
    }
}