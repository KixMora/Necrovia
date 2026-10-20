using UnityEngine;

public class FloatingScrapTextSpawner : MonoBehaviour
{
    public static FloatingScrapTextSpawner Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera worldCamera;
    [SerializeField] private RectTransform uiRoot;
    [SerializeField] private FloatingScrapText floatingTextPrefab;

    private void Awake()
    {
        Instance = this;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (worldCamera == null)
            worldCamera = Camera.main;

        if (uiRoot == null && canvas != null)
            uiRoot = canvas.transform as RectTransform;
    }

    public void Show(Vector3 worldPosition, int amount)
    {
        if (canvas == null || uiRoot == null || floatingTextPrefab == null || worldCamera == null)
            return;

        Vector3 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiRoot,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        FloatingScrapText text = Instantiate(floatingTextPrefab, uiRoot);
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchoredPosition = localPoint;

        text.Setup(amount);
    }
}