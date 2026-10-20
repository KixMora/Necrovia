using UnityEngine;
using UnityEngine.UI;

public class MissionConnectionLineUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private RectTransform rootRect;

    [Header("Visuals")]
    [SerializeField] private RectTransform glowRect;
    [SerializeField] private RectTransform coreRect;
    [SerializeField] private Image glowImage;
    [SerializeField] private Image coreImage;

    [Header("Thickness")]
    [SerializeField] private float glowThickness = 10f;
    [SerializeField] private float coreThickness = 4f;

    [Header("Unlocked Colors")]
    [SerializeField] private Color unlockedGlowColor = new Color32(80, 255, 120, 90);
    [SerializeField] private Color unlockedCoreColor = new Color32(180, 255, 190, 220);

    [Header("Locked Colors")]
    [SerializeField] private Color lockedGlowColor = new Color32(160, 60, 60, 70);
    [SerializeField] private Color lockedCoreColor = new Color32(220, 90, 90, 160);

    [Header("Pulse Settings")]
    [SerializeField] private bool pulseGlow = true;
    [SerializeField] private float unlockedPulseSpeed = 1.8f;
    [SerializeField] private float lockedPulseSpeed = 1.2f;
    [SerializeField] private float unlockedPulseStrength = 0.35f;
    [SerializeField] private float lockedPulseStrength = 0.2f;

    private bool isUnlocked;
    private Color baseGlowColor;
    private Color baseCoreColor;
    private float randomOffset;

    public void Setup(Vector2 start, Vector2 end, bool unlocked)
    {
        if (rootRect == null)
            rootRect = GetComponent<RectTransform>();

        isUnlocked = unlocked;
        randomOffset = Random.Range(0f, 10f);

        Vector2 direction = end - start;
        float length = direction.magnitude;
        Vector2 center = (start + end) * 0.5f;

        rootRect.anchoredPosition = center;
        rootRect.sizeDelta = new Vector2(length, glowThickness);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rootRect.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (glowRect != null)
        {
            glowRect.anchoredPosition = Vector2.zero;
            glowRect.localRotation = Quaternion.identity;
            glowRect.localScale = Vector3.one;
            glowRect.sizeDelta = new Vector2(length, glowThickness);
        }

        if (coreRect != null)
        {
            coreRect.anchoredPosition = Vector2.zero;
            coreRect.localRotation = Quaternion.identity;
            coreRect.localScale = Vector3.one;
            coreRect.sizeDelta = new Vector2(length, coreThickness);
        }

        baseGlowColor = unlocked ? unlockedGlowColor : lockedGlowColor;
        baseCoreColor = unlocked ? unlockedCoreColor : lockedCoreColor;

        if (glowImage != null)
            glowImage.color = baseGlowColor;

        if (coreImage != null)
            coreImage.color = baseCoreColor;
    }

    private void Update()
    {
        if (!pulseGlow || glowImage == null)
            return;

        float speed = isUnlocked ? unlockedPulseSpeed : lockedPulseSpeed;
        float strength = isUnlocked ? unlockedPulseStrength : lockedPulseStrength;

        float wave = (Mathf.Sin((Time.unscaledTime + randomOffset) * speed) + 1f) * 0.5f;

        Color glowColor = baseGlowColor;
        float baseAlpha = baseGlowColor.a;
        glowColor.a = Mathf.Clamp01(baseAlpha * Mathf.Lerp(1f - strength, 1f + strength, wave));
        glowImage.color = glowColor;
    }
}