using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingScrapText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text text;

    [Header("Animation")]
    [SerializeField] private float moveUpDistance = 70f;
    [SerializeField] private float lifeTime = 0.8f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (text == null)
            text = GetComponent<TMP_Text>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(int amount)
    {
        if (text != null)
            text.text = "+" + amount;

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * moveUpDistance;

        float timer = 0f;

        while (timer < lifeTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / lifeTime;

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}