using UnityEngine;

public class ZombieGroupVisual : MonoBehaviour
{
    [Header("Floating Motion")]
    public float bobHeight = 0.05f;
    public float bobSpeed = 2f;

    [Header("Sway Motion")]
    public float swayAngle = 5f;
    public float swaySpeed = 2.5f;

    private Vector3 startLocalPos;
    private Quaternion startLocalRot;
    private float randomOffset;

    private void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;
        randomOffset = Random.Range(0f, 10f);
    }

    private void Update()
    {
        float t = Time.time + randomOffset;

        float yOffset = Mathf.Sin(t * bobSpeed) * bobHeight;
        float zRot = Mathf.Sin(t * swaySpeed) * swayAngle;

        transform.localPosition = startLocalPos + new Vector3(0f, yOffset, 0f);
        transform.localRotation = startLocalRot * Quaternion.Euler(0f, 0f, zRot);
    }
}