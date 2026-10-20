using UnityEngine;

public class MissionRuntimeTracker : MonoBehaviour
{
    public static MissionRuntimeTracker Instance { get; private set; }

    public int ZombiesKilled { get; private set; }
    public int WavesSurvived { get; private set; }
    public float TimeSurvived { get; private set; }

    public bool MissionCompleted { get; private set; }

    private MissionData currentMission;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (MissionManager.Instance != null)
            currentMission = MissionManager.Instance.CurrentMission;

        Debug.Log("[MissionRuntimeTracker] Current mission = " + (currentMission != null ? currentMission.missionTitle : "NULL"));
    }

    private void Update()
    {
        if (MissionCompleted)
            return;

        TimeSurvived += Time.deltaTime;
        CheckObjectives();
    }

    public void AddZombieKill(int amount = 1)
    {
        if (MissionCompleted)
            return;

        ZombiesKilled += amount;
        Debug.Log("[MissionRuntimeTracker] ZombiesKilled = " + ZombiesKilled);
        CheckObjectives();
    }

    public void SetWaveSurvived(int wave)
    {
        if (MissionCompleted)
            return;

        WavesSurvived = Mathf.Max(WavesSurvived, wave);
        Debug.Log("[MissionRuntimeTracker] WavesSurvived = " + WavesSurvived);
        CheckObjectives();
    }

    private void CheckObjectives()
    {
        if (currentMission == null || currentMission.objectives == null || currentMission.objectives.Count == 0)
            return;

        bool result = currentMission.completionMode == MissionCompletionMode.AllObjectivesRequired;

        foreach (var objective in currentMission.objectives)
        {
            bool completed = IsObjectiveCompleted(objective);

            if (currentMission.completionMode == MissionCompletionMode.AllObjectivesRequired)
            {
                if (!completed)
                {
                    result = false;
                    break;
                }
            }
            else
            {
                if (completed)
                {
                    result = true;
                    break;
                }

                result = false;
            }
        }

        if (result)
            CompleteMission();
    }

    private bool IsObjectiveCompleted(MissionObjective objective)
    {
        switch (objective.type)
        {
            case MissionObjectiveType.KillZombies:
                return ZombiesKilled >= objective.targetValue;

            case MissionObjectiveType.SurviveTime:
                return TimeSurvived >= objective.targetValue;

            case MissionObjectiveType.SurviveWaves:
                return WavesSurvived >= objective.targetValue;

            default:
                return false;
        }
    }

    private void CompleteMission()
    {
        MissionCompleted = true;

        Debug.Log("[MissionRuntimeTracker] Mission completed!");

        if (MissionManager.Instance != null && MissionManager.Instance.SelectedMission != null)
        {
            if (AnalyticsManager.Instance != null)
                AnalyticsManager.Instance.TrackMissionComplete(MissionManager.Instance.SelectedMission);

            AnalyticsManager.Instance?.TrackZombieKills(
                MissionManager.Instance.SelectedMission,
                ZombiesKilled
            );

            MissionManager.Instance.CompleteMission(MissionManager.Instance.SelectedMission);
        }

        if (MissionResultUI.Instance != null)
        {
            MissionResultUI.Instance.ShowCompleted();
        }
    }
}