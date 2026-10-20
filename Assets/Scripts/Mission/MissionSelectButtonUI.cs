using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionSelectButtonUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private Button selectButton;

    private MissionData missionData;
    private MissionSelectUI owner;

    public void Setup(MissionData data, bool unlocked, MissionSelectUI missionSelectUI)
    {
        missionData = data;
        owner = missionSelectUI;

        if (titleText != null)
            titleText.text = data.missionTitle;

        if (descriptionText != null)
            descriptionText.text = data.description;

        if (previewImage != null)
            previewImage.sprite = data.icon;

        if (lockedOverlay != null)
            lockedOverlay.SetActive(!unlocked);

        if (selectButton != null)
            selectButton.interactable = unlocked;
    }

    public void OnClickSelect()
    {
        if (missionData == null || owner == null)
            return;

        owner.SelectMission(missionData);
    }
}