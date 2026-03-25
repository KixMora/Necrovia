using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 12f, -12f);
    public float smoothTime = 0.08f;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}
