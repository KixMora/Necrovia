using UnityEngine;
using GameAnalyticsSDK;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TrackMissionStart(MissionData mission)
    {
        if (mission == null)
            return;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, mission.missionId);
        Debug.Log("[Analytics] mission_start -> " + mission.missionId);
    }

    public void TrackMissionComplete(MissionData mission)
    {
        if (mission == null)
            return;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, mission.missionId);
        Debug.Log("[Analytics] mission_complete -> " + mission.missionId);
    }

    public void TrackMissionFail(MissionData mission)
    {
        if (mission == null)
            return;

        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, mission.missionId);
        Debug.Log("[Analytics] mission_failed -> " + mission.missionId);
    }

    public void TrackWaveReached(MissionData mission, int wave)
    {
        if (mission == null)
            return;

        GameAnalytics.NewDesignEvent($"wave:reached:{mission.missionId}", wave);
        Debug.Log("[Analytics] wave:reached -> " + mission.missionId + " wave=" + wave);
    }

    public void TrackZombieKills(MissionData mission, int kills)
    {
        if (mission == null)
            return;

        GameAnalytics.NewDesignEvent($"zombies:killed:{mission.missionId}", kills);
        Debug.Log("[Analytics] zombies:killed -> " + mission.missionId + " kills=" + kills);
    }
}