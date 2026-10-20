using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletTracer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Visual")]
    [SerializeField] private float startWidth = 0.045f;
    [SerializeField] private float endWidth = 0.015f;

    [Header("Color")]
    [SerializeField] private Color startColor = new Color(1f, 0.95f, 0.45f, 1f);
    [SerializeField] private Color endColor = new Color(1f, 0.25f, 0.05f, 0.25f);

    [Header("Fade")]
    [SerializeField] private bool fadeOut = true;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        SetupLineDefaults();
    }

    private void SetupLineDefaults()
    {
        if (lineRenderer == null)
            return;

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;

        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }

    public void Setup(Vector3 start, Vector3 end, float lifeTime)
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            Destroy(gameObject, lifeTime);
            return;
        }

        SetupLineDefaults();

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (fadeOut)
            fadeRoutine = StartCoroutine(FadeAndDestroy(lifeTime));
        else
            Destroy(gameObject, lifeTime);
    }

    private IEnumerator FadeAndDestroy(float lifeTime)
    {
        float timer = 0f;

        Color originalStart = startColor;
        Color originalEnd = endColor;

        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / lifeTime);

            Color fadedStart = originalStart;
            Color fadedEnd = originalEnd;

            fadedStart.a = Mathf.Lerp(originalStart.a, 0f, t);
            fadedEnd.a = Mathf.Lerp(originalEnd.a, 0f, t);

            lineRenderer.startColor = fadedStart;
            lineRenderer.endColor = fadedEnd;

            yield return null;
        }

        Destroy(gameObject);
    }
}