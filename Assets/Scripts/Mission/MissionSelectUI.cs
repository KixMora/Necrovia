using System.Collections.Generic;
using UnityEngine;

public class MissionSelectUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<MissionData> missions = new List<MissionData>();

    [Header("UI")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private MissionSelectButtonUI missionButtonPrefab;

    private void Start()
    {
        BuildMissionList();
    }

    public void BuildMissionList()
    {
        if (contentParent == null)
        {
            Debug.LogError("MissionSelectUI: contentParent is null.");
            return;
        }

        if (missionButtonPrefab == null)
        {
            Debug.LogError("MissionSelectUI: missionButtonPrefab is null.");
            return;
        }

        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }

        foreach (MissionData mission in missions)
        {
            if (mission == null)
                continue;

            MissionSelectButtonUI item = Instantiate(missionButtonPrefab, contentParent);

            bool unlocked = false;

            if (MissionManager.Instance != null)
                unlocked = MissionManager.Instance.IsMissionUnlocked(mission);

            item.Setup(mission, unlocked, this);
        }
    }

    public void SelectMission(MissionData mission)
    {
        if (mission == null)
        {
            Debug.LogError("MissionSelectUI: mission is null.");
            return;
        }

        if (MissionManager.Instance == null)
        {
            Debug.LogError("MissionSelectUI: MissionManager.Instance is null.");
            return;
        }

        if (string.IsNullOrWhiteSpace(mission.sceneName))
        {
            Debug.LogError("MissionSelectUI: mission.sceneName is empty for mission " + mission.missionTitle);
            return;
        }

        MissionManager.Instance.SelectMission(mission);

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.LoadScene(mission.sceneName);
        else
            Debug.LogError("MissionSelectUI: SceneTransitionManager.Instance is null.");
    }
}