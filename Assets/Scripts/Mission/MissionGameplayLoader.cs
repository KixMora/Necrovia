using TMPro;
using UnityEngine;

public class MissionGameplayLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text missionTitleText;
    [SerializeField] private TMP_Text missionDescriptionText;

    private void Start()
    {
        if (MissionManager.Instance == null || MissionManager.Instance.SelectedMission == null)
        {
            Debug.LogWarning("MissionGameplayLoader: No mission selected.");
            return;
        }

        MissionData mission = MissionManager.Instance.SelectedMission;

        if (missionTitleText != null)
            missionTitleText.text = mission.missionTitle;

        if (missionDescriptionText != null)
            missionDescriptionText.text = mission.description;

        Debug.Log("Loaded mission: " + mission.missionTitle);
    }
}