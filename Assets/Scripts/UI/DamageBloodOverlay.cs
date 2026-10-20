using System.Collections;
using UnityEngine;

public class DamageBloodOverlay : MonoBehaviour
{
    public static DamageBloodOverlay Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Effect")]
    [SerializeField] private float maxAlpha = 0.75f;
    [SerializeField] private float fadeInTime = 0.08f;
    [SerializeField] private float holdTime = 0.05f;
    [SerializeField] private float fadeOutTime = 0.45f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        Instance = this;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowDamage()
    {
        if (canvasGroup == null)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        yield return FadeTo(maxAlpha, fadeInTime);

        if (holdTime > 0f)
            yield return new WaitForSecondsRealtime(holdTime);

        yield return FadeTo(0f, fadeOutTime);

        currentRoutine = null;
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}