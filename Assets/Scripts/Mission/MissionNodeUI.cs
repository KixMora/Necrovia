using UnityEngine;
using UnityEngine.UI;

public class MissionNodeUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private Button button;
    [SerializeField] private MissionNodeGlow glow;
    [SerializeField] private Image completedMark;

    private MissionData missionData;
    private MissionMapUI owner;
    private bool isUnlocked;

    public void Setup(MissionData data, bool unlocked, MissionMapUI mapUI)
    {
        missionData = data;
        owner = mapUI;
        isUnlocked = unlocked;

        if (glow != null)
        {
            glow.SetUnlockedState(unlocked);
            glow.SetSelectedState(false);
        }

        if (icon != null && data != null && data.icon != null)
            icon.sprite = data.icon;

        if (lockedOverlay != null)
            lockedOverlay.SetActive(!unlocked);

        bool completed = MissionManager.Instance != null && MissionManager.Instance.IsMissionCompleted(data);

        if (completedMark != null)
        {
            Color c = completedMark.color;
            c.a = completed ? 1f : 0f;
            completedMark.color = c;
            completedMark.raycastTarget = false;
        }

        if (glow != null)
            glow.SetCompletedState(completed);

        if (button == null)
            return;

        button.interactable = unlocked;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (missionData == null || owner == null)
            return;

        owner.SelectMission(missionData);
    }

    public void SetSelected(bool selected)
    {
        if (!isUnlocked)
            selected = false;

        if (glow != null)
            glow.SetSelectedState(selected);

        transform.localScale = selected ? Vector3.one * 1.08f : Vector3.one;
    }

    public bool HasMission(MissionData data)
    {
        return missionData == data;
    }
}