using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    public MissionData SelectedMission { get; private set; }
    public MissionData CurrentMission => SelectedMission;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[MissionManager] Initialized");
    }

    public bool IsMissionUnlocked(MissionData mission)
    {
        if (mission == null)
        {
            Debug.LogWarning("[MissionManager] IsMissionUnlocked called with NULL mission");
            return false;
        }

        string key = GetUnlockedKey(mission);

        if (PlayerPrefs.HasKey(key))
        {
            bool savedUnlocked = PlayerPrefs.GetInt(key) == 1;
            Debug.Log($"[MissionManager] {mission.missionTitle} -> loaded from PlayerPrefs = {savedUnlocked}");
            return savedUnlocked;
        }

        if (mission.unlockedByDefault)
        {
            Debug.Log($"[MissionManager] {mission.missionTitle} -> unlocked by default");
            return true;
        }

        if (mission.requiredMission != null)
        {
            bool requiredCompleted = IsMissionCompleted(mission.requiredMission);
            Debug.Log($"[MissionManager] {mission.missionTitle} -> required mission completed = {requiredCompleted}");
            return requiredCompleted;
        }

        Debug.Log($"[MissionManager] {mission.missionTitle} -> locked");
        return false;
    }

    public bool IsMissionCompleted(MissionData mission)
    {
        if (mission == null)
        {
            Debug.LogWarning("[MissionManager] IsMissionCompleted called with NULL mission");
            return false;
        }

        string key = GetCompletedKey(mission);
        bool completed = PlayerPrefs.GetInt(key, 0) == 1;

        Debug.Log($"[MissionManager] IsMissionCompleted -> {mission.missionTitle} = {completed}, key = {key}");
        return completed;
    }

    public void SelectMission(MissionData mission)
    {
        SelectedMission = mission;
        Debug.Log("[MissionManager] Selected mission: " + (mission != null ? mission.missionTitle : "NULL"));
    }

    public void CompleteMission(MissionData mission)
    {
        if (mission == null)
        {
            Debug.LogWarning("[MissionManager] CompleteMission called with NULL mission");
            return;
        }

        string completedKey = GetCompletedKey(mission);
        string unlockedKey = GetUnlockedKey(mission);

        PlayerPrefs.SetInt(completedKey, 1);
        PlayerPrefs.SetInt(unlockedKey, 1);
        PlayerPrefs.Save();

        Debug.Log($"[MissionManager] Mission completed: {mission.missionTitle}");
        Debug.Log($"[MissionManager] Saved completedKey = {completedKey}");
        Debug.Log($"[MissionManager] Saved unlockedKey = {unlockedKey}");
    }

    public void UnlockMission(MissionData mission)
    {
        if (mission == null)
            return;

        PlayerPrefs.SetInt(GetUnlockedKey(mission), 1);
        PlayerPrefs.Save();

        Debug.Log("[MissionManager] Mission unlocked manually: " + mission.missionTitle);
    }

    [ContextMenu("Reset Mission Progress")]
    public void ResetMissionProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[MissionManager] All PlayerPrefs deleted");
    }

    private string GetUnlockedKey(MissionData mission)
    {
        return $"mission_{mission.missionId}_unlocked";
    }

    private string GetCompletedKey(MissionData mission)
    {
        return $"mission_{mission.missionId}_completed";
    }
}