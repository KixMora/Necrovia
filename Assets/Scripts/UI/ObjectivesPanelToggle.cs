using UnityEngine;
using UnityEngine.UI;

public class ObjectivesPanelToggle : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject objectivesPanel;

    [Header("Button Image")]
    [SerializeField] private Image buttonImage;

    [Header("Sprites")]
    [SerializeField] private Sprite openedSprite; // спрайт, когда панель открыта
    [SerializeField] private Sprite closedSprite; // спрайт, когда панель закрыта

    [Header("Start State")]
    [SerializeField] private bool startOpened = true;

    private bool isOpened;

    private void Start()
    {
        isOpened = startOpened;
        ApplyState();
    }

    public void Toggle()
    {
        isOpened = !isOpened;
        ApplyState();
    }

    public void Show()
    {
        isOpened = true;
        ApplyState();
    }

    public void Hide()
    {
        isOpened = false;
        ApplyState();
    }

    private void ApplyState()
    {
        if (objectivesPanel != null)
            objectivesPanel.SetActive(isOpened);

        if (buttonImage != null)
        {
            if (isOpened && openedSprite != null)
                buttonImage.sprite = openedSprite;
            else if (!isOpened && closedSprite != null)
                buttonImage.sprite = closedSprite;
        }
    }
}