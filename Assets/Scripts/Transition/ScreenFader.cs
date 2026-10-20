using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [Header("References")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float fadeInDelayAfterSceneLoad = 0.15f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        Debug.Log("[ScreenFader] Awake on " + gameObject.name);

        if (Instance != null && Instance != this)
        {
            Debug.Log("[ScreenFader] Duplicate destroyed: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        Debug.Log("[ScreenFader] Start");

        if (fadeCanvasGroup == null)
        {
            Debug.LogError("[ScreenFader] fadeCanvasGroup is NOT assigned.");
            return;
        }

        Debug.Log("[ScreenFader] fadeCanvasGroup found on: " + fadeCanvasGroup.gameObject.name);

        // На старте держим экран закрытым
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;

        StartFadeIn();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[ScreenFader] Scene loaded: " + scene.name);

        if (fadeCanvasGroup == null)
        {
            Debug.LogError("[ScreenFader] fadeCanvasGroup is NULL on scene load.");
            return;
        }

        // Сразу закрываем экран, чтобы новая сцена не успела мигнуть
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;

        StartCoroutine(DelayedFadeIn());
    }

    private IEnumerator DelayedFadeIn()
    {
        Debug.Log("[ScreenFader] DelayedFadeIn start");

        // Даём сцене 1 кадр полностью собраться
        yield return null;

        // Небольшая пауза перед осветлением
        yield return new WaitForSecondsRealtime(fadeInDelayAfterSceneLoad);

        StartFadeIn();
    }

    public void StartFadeIn()
    {
        Debug.Log("[ScreenFader] StartFadeIn");

        if (fadeCanvasGroup == null)
        {
            Debug.LogError("[ScreenFader] StartFadeIn failed: fadeCanvasGroup is NULL");
            return;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(1f, 0f));
    }

    public IEnumerator FadeOut()
    {
        Debug.Log("[ScreenFader] FadeOut");

        if (fadeCanvasGroup == null)
        {
            Debug.LogError("[ScreenFader] FadeOut failed: fadeCanvasGroup is NULL");
            yield break;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(0f, 1f));
        yield return fadeRoutine;
    }

    private IEnumerator FadeRoutine(float from, float to)
    {
        Debug.Log($"[ScreenFader] FadeRoutine {from} -> {to}");

        fadeCanvasGroup.alpha = from;
        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = true;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;

        bool hidden = to <= 0.01f;
        fadeCanvasGroup.blocksRaycasts = !hidden;
        fadeCanvasGroup.interactable = !hidden;

        Debug.Log("[ScreenFader] Fade finished. Alpha = " + fadeCanvasGroup.alpha);

        fadeRoutine = null;
    }
}