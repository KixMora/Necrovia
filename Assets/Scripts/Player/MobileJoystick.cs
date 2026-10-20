using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [SerializeField] private float handleRange = 80f;
    [SerializeField] private float deadZone = 0.15f;

    public Vector2 Direction { get; private set; }
    public bool IsPressed { get; private set; }

    private Vector2 startPosition;

    private void Awake()
    {
        if (background == null)
            background = transform as RectTransform;

        if (handle != null)
            startPosition = handle.anchoredPosition;

        Direction = Vector2.zero;
        IsPressed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (background == null || handle == null)
            return;

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        Vector2 clamped = Vector2.ClampMagnitude(localPoint, handleRange);

        handle.anchoredPosition = startPosition + clamped;

        Vector2 rawDirection = clamped / handleRange;

        if (rawDirection.magnitude < deadZone)
            Direction = Vector2.zero;
        else
            Direction = rawDirection;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        Direction = Vector2.zero;

        if (handle != null)
            handle.anchoredPosition = startPosition;
    }
}