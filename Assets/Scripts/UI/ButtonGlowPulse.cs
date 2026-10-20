using UnityEngine;
using UnityEngine.UI;

public class ButtonGlowPulse : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image glowImage;

    [Header("Alpha Pulse")]
    [SerializeField] private float minAlpha = 0.25f;
    [SerializeField] private float maxAlpha = 0.75f;
    [SerializeField] private float pulseSpeed = 1.6f;

    private Color baseColor;

    private void Awake()
    {
        if (glowImage == null)
            glowImage = GetComponent<Image>();

        if (glowImage != null)
            baseColor = glowImage.color;
    }

    private void Update()
    {
        if (glowImage == null)
            return;

        float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) * 0.5f;

        Color c = baseColor;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        glowImage.color = c;
    }
}