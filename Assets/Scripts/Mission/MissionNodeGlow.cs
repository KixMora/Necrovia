using UnityEngine;
using UnityEngine.UI;

public class MissionNodeGlow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform glowOuter;
    [SerializeField] private RectTransform glowInner;
    [SerializeField] private Image glowOuterImage;
    [SerializeField] private Image glowInnerImage;

    [Header("Unlocked Colors")]
    [SerializeField] private Color unlockedOuterColor = new Color32(60, 210, 100, 90);
    [SerializeField] private Color unlockedInnerColor = new Color32(150, 255, 180, 220);

    [Header("Locked Colors")]
    [SerializeField] private Color lockedOuterColor = new Color32(180, 40, 40, 90);
    [SerializeField] private Color lockedInnerColor = new Color32(255, 90, 90, 220);

    [Header("Selected Colors")]
    [SerializeField] private Color selectedOuterColor = new Color32(255, 220, 120, 120);
    [SerializeField] private Color selectedInnerColor = new Color32(255, 245, 180, 255);

    [Header("Completed Colors")]
    [SerializeField] private Color completedOuterColor = new Color32(80, 230, 120, 100);
    [SerializeField] private Color completedInnerColor = new Color32(190, 255, 210, 230);

    [Header("Unlocked Pulse")]
    [SerializeField] private float unlockedPulseSpeed = 2f;
    [SerializeField] private float unlockedOuterScaleMin = 0.9f;
    [SerializeField] private float unlockedOuterScaleMax = 1.15f;
    [SerializeField] private float unlockedInnerScaleMin = 0.95f;
    [SerializeField] private float unlockedInnerScaleMax = 1.08f;

    [Header("Locked Pulse")]
    [SerializeField] private float lockedPulseSpeed = 3f;
    [SerializeField] private float lockedOuterScaleMin = 0.92f;
    [SerializeField] private float lockedOuterScaleMax = 1.1f;
    [SerializeField] private float lockedInnerScaleMin = 0.97f;
    [SerializeField] private float lockedInnerScaleMax = 1.05f;

    [Header("Selected Pulse")]
    [SerializeField] private float selectedPulseSpeed = 2.8f;
    [SerializeField] private float selectedOuterScaleMin = 1.05f;
    [SerializeField] private float selectedOuterScaleMax = 1.28f;
    [SerializeField] private float selectedInnerScaleMin = 1.05f;
    [SerializeField] private float selectedInnerScaleMax = 1.18f;

    [Header("Completed Pulse")]
    [SerializeField] private float completedPulseSpeed = 1.6f;
    [SerializeField] private float completedOuterScaleMin = 0.95f;
    [SerializeField] private float completedOuterScaleMax = 1.12f;
    [SerializeField] private float completedInnerScaleMin = 0.98f;
    [SerializeField] private float completedInnerScaleMax = 1.08f;

    [Header("Alpha")]
    [SerializeField] private float outerAlphaMin = 0.2f;
    [SerializeField] private float outerAlphaMax = 0.5f;
    [SerializeField] private float innerAlphaMin = 0.65f;
    [SerializeField] private float innerAlphaMax = 1f;

    private bool isUnlocked = true;
    private bool isSelected = false;
    private bool isCompleted = false;

    public void SetUnlockedState(bool unlocked)
    {
        isUnlocked = unlocked;

        if (!isUnlocked)
        {
            isSelected = false;
            isCompleted = false;
        }

        ApplyBaseColors();
    }

    public void SetSelectedState(bool selected)
    {
        if (!isUnlocked)
        {
            isSelected = false;
            ApplyBaseColors();
            return;
        }

        isSelected = selected;
        ApplyBaseColors();
    }

    public void SetCompletedState(bool completed)
    {
        isCompleted = completed;
        ApplyBaseColors();
    }

    private void Awake()
    {
        ApplyBaseColors();
    }

    private void Update()
    {
        float pulseSpeed;
        float outerMin;
        float outerMax;
        float innerMin;
        float innerMax;

        // ПОРЯДОК ВАЖЕН:
        // locked > selected > completed > unlocked
        if (!isUnlocked)
        {
            pulseSpeed = lockedPulseSpeed;
            outerMin = lockedOuterScaleMin;
            outerMax = lockedOuterScaleMax;
            innerMin = lockedInnerScaleMin;
            innerMax = lockedInnerScaleMax;
        }
        else if (isSelected)
        {
            pulseSpeed = selectedPulseSpeed;
            outerMin = selectedOuterScaleMin;
            outerMax = selectedOuterScaleMax;
            innerMin = selectedInnerScaleMin;
            innerMax = selectedInnerScaleMax;
        }
        else if (isCompleted)
        {
            pulseSpeed = completedPulseSpeed;
            outerMin = completedOuterScaleMin;
            outerMax = completedOuterScaleMax;
            innerMin = completedInnerScaleMin;
            innerMax = completedInnerScaleMax;
        }
        else
        {
            pulseSpeed = unlockedPulseSpeed;
            outerMin = unlockedOuterScaleMin;
            outerMax = unlockedOuterScaleMax;
            innerMin = unlockedInnerScaleMin;
            innerMax = unlockedInnerScaleMax;
        }

        float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;

        if (glowOuter != null)
        {
            float scale = Mathf.Lerp(outerMin, outerMax, t);
            glowOuter.localScale = Vector3.one * scale;
        }

        if (glowInner != null)
        {
            float scale = Mathf.Lerp(innerMin, innerMax, 1f - t);
            glowInner.localScale = Vector3.one * scale;
        }

        if (glowOuterImage != null)
        {
            Color c = GetOuterColor();
            c.a = Mathf.Lerp(outerAlphaMin, outerAlphaMax, t);
            glowOuterImage.color = c;
        }

        if (glowInnerImage != null)
        {
            Color c = GetInnerColor();
            c.a = Mathf.Lerp(innerAlphaMin, innerAlphaMax, 1f - t);
            glowInnerImage.color = c;
        }
    }

    private void ApplyBaseColors()
    {
        if (glowOuterImage != null)
            glowOuterImage.color = GetOuterColor();

        if (glowInnerImage != null)
            glowInnerImage.color = GetInnerColor();
    }

    private Color GetOuterColor()
    {
        if (!isUnlocked) return lockedOuterColor;
        if (isSelected) return selectedOuterColor;
        if (isCompleted) return completedOuterColor;
        return unlockedOuterColor;
    }

    private Color GetInnerColor()
    {
        if (!isUnlocked) return lockedInnerColor;
        if (isSelected) return selectedInnerColor;
        if (isCompleted) return completedInnerColor;
        return unlockedInnerColor;
    }
}