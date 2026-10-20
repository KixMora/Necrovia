using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "Necrovia/Mission Data")]
public class MissionData : ScriptableObject
{
    private const string LanguageKey = "selected_language";

    [Header("ID")]
    public string missionId;

    [Header("Texts - Fallback")]
    public string missionTitle;

    [TextArea(3, 6)]
    public string description;

    [Header("Texts - Russian")]
    public string missionTitleRu;

    [TextArea(3, 6)]
    public string descriptionRu;

    [Header("Texts - English")]
    public string missionTitleEn;

    [TextArea(3, 6)]
    public string descriptionEn;

    [Header("Scene")]
    public string sceneName;

    [Header("Map")]
    public Vector2 mapPosition;
    public Sprite icon;

    [Header("Unlock")]
    public bool unlockedByDefault;
    public MissionData requiredMission;

    [Header("Objectives")]
    public MissionCompletionMode completionMode = MissionCompletionMode.AllObjectivesRequired;
    public List<MissionObjective> objectives = new List<MissionObjective>();

    public Sprite previewImage => icon;

    public string GetTitle()
    {
        if (IsRussian())
        {
            if (!string.IsNullOrWhiteSpace(missionTitleRu))
                return missionTitleRu;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(missionTitleEn))
                return missionTitleEn;
        }

        return missionTitle;
    }

    public string GetDescription()
    {
        if (IsRussian())
        {
            if (!string.IsNullOrWhiteSpace(descriptionRu))
                return descriptionRu;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(descriptionEn))
                return descriptionEn;
        }

        return description;
    }

    private bool IsRussian()
    {
        if (LocalizationSettings.SelectedLocale != null)
            return LocalizationSettings.SelectedLocale.Identifier.Code == "ru";

        string lang = PlayerPrefs.GetString(LanguageKey, "ru");
        return lang == "ru";
    }
}