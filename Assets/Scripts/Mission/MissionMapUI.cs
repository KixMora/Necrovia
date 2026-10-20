using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionMapUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<MissionData> missions;

    [Header("UI")]
    [SerializeField] private RectTransform nodesRoot;
    [SerializeField] private MissionNodeUI nodePrefab;

    [Header("Right Panel")]
    [SerializeField] private GameObject missionInfoPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Connections")]
    [SerializeField] private RectTransform linesRoot;
    [SerializeField] private MissionConnectionLineUI linePrefab;
    private MissionData selectedMission;
    private readonly List<MissionNodeUI> createdNodes = new List<MissionNodeUI>();
    private readonly Dictionary<MissionData, MissionNodeUI> nodeByMission = new Dictionary<MissionData, MissionNodeUI>();

    private void Start()
    {
        if (missionInfoPanel != null)
            missionInfoPanel.SetActive(false);

        BuildMap();
        AutoSelectFirstUnlockedMission();
    }
    private void AutoSelectFirstUnlockedMission()
    {
        if (missions == null || missions.Count == 0)
            return;

        for (int i = 0; i < missions.Count; i++)
        {
            MissionData mission = missions[i];

            if (mission == null)
                continue;

            if (MissionManager.Instance != null && MissionManager.Instance.IsMissionUnlocked(mission))
            {
                SelectMission(mission);
                Debug.Log("[MissionMapUI] Auto-selected first unlocked mission: " + mission.missionTitle);
                return;
            }
        }

        Debug.LogWarning("[MissionMapUI] No unlocked mission found for auto-select.");
    }
    public void BackToMainMenu()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("[MissionMapUI] SceneTransitionManager.Instance is NULL, loading directly.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
    private void BuildMap()
    {
        Debug.Log("[MissionMapUI] BuildMap started");
        nodeByMission.Clear();
        if (MissionManager.Instance == null)
        {
            Debug.LogError("[MissionMapUI] MissionManager.Instance is NULL. Add MissionManager to the first scene.");
            return;
        }

        if (nodesRoot == null)
        {
            Debug.LogError("[MissionMapUI] nodesRoot is NULL");
            return;
        }

        if (nodePrefab == null)
        {
            Debug.LogError("[MissionMapUI] nodePrefab is NULL");
            return;
        }

        createdNodes.Clear();

        foreach (Transform child in nodesRoot)
        {
            Destroy(child.gameObject);
        }

        foreach (var mission in missions)
        {
            if (mission == null)
                continue;

            var node = Instantiate(nodePrefab, nodesRoot);

            RectTransform nodeRect = node.GetComponent<RectTransform>();
            if (nodeRect != null)
                nodeRect.anchoredPosition = mission.mapPosition;

            bool unlocked = MissionManager.Instance.IsMissionUnlocked(mission);

            Debug.Log($"[MissionMapUI] Mission {mission.missionTitle} unlocked = {unlocked}");

            node.Setup(mission, unlocked, this);

            createdNodes.Add(node);

            nodeByMission[mission] = node;
        }
        
        BuildConnections();
    }

    private void BuildConnections()
    {
        if (linesRoot == null || linePrefab == null)
        {
            Debug.LogWarning("[MissionMapUI] linesRoot or linePrefab is NULL");
            return;
        }

        foreach (Transform child in linesRoot)
            Destroy(child.gameObject);

        foreach (var mission in missions)
        {
            if (mission == null || mission.requiredMission == null)
                continue;

            if (!nodeByMission.ContainsKey(mission))
                continue;

            if (!nodeByMission.ContainsKey(mission.requiredMission))
                continue;

            RectTransform fromRect = nodeByMission[mission.requiredMission].GetComponent<RectTransform>();
            RectTransform toRect = nodeByMission[mission].GetComponent<RectTransform>();

            if (fromRect == null || toRect == null)
                continue;

            bool unlocked = MissionManager.Instance != null && MissionManager.Instance.IsMissionUnlocked(mission);

            MissionConnectionLineUI line = Instantiate(linePrefab, linesRoot);
            line.Setup(fromRect.anchoredPosition, toRect.anchoredPosition, unlocked);
        }
    }
    private bool IsNodeForMission(MissionNodeUI node, MissionData mission)
    {
        return node != null && node.HasMission(mission);
    }
    public void SelectMission(MissionData mission)
    {
        Debug.Log("[MissionMapUI] SelectMission: " + mission.missionTitle);

        selectedMission = mission;

        for (int i = 0; i < createdNodes.Count; i++)
        {
            if (createdNodes[i] == null)
                continue;

            bool isSelected = createdNodes[i].name.Contains(mission.name);
            createdNodes[i].SetSelected(false);
        }

        for (int i = 0; i < createdNodes.Count; i++)
        {
            if (createdNodes[i] == null)
                continue;

            createdNodes[i].SetSelected(IsNodeForMission(createdNodes[i], mission));
        }

        if (missionInfoPanel != null)
            missionInfoPanel.SetActive(true);

        if (titleText != null)
            titleText.text = mission.GetTitle();

        if (descriptionText != null)
            descriptionText.text = mission.GetDescription();
    }

    public void StartMission()
    {
        if (selectedMission == null)
        {
            Debug.LogWarning("[MissionMapUI] StartMission called but selectedMission is NULL");
            return;
        }

        if (MissionManager.Instance == null)
        {
            Debug.LogError("[MissionMapUI] MissionManager.Instance is NULL");
            return;
        }

        MissionManager.Instance.SelectMission(selectedMission);

        if (AnalyticsManager.Instance != null)
            AnalyticsManager.Instance.TrackMissionStart(selectedMission);

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(selectedMission.sceneName);
        }
        else
        {
            Debug.LogError("[MissionMapUI] SceneTransitionManager.Instance is NULL");
        }
    }

    public void RebuildMap()
    {
        BuildMap();
    }
}