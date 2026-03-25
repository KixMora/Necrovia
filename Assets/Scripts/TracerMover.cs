using UnityEngine;

public class TracerMover : MonoBehaviour
{
    public float speed = 80f;      // скорость "пули"
    public float lifeTime = 0.25f; // на всякий случай авто-удаление

    private Vector3 start;
    private Vector3 end;
    private Vector3 dir;
    private float dist;
    private float traveled;

    public void Init(Vector3 startPos, Vector3 endPos, float tracerSpeed)
    {
        start = startPos;
        end = endPos;

        Vector3 d = (end - start);
        d.y = 0f;

        dist = d.magnitude;
        dir = dist > 0.0001f ? d / dist : Vector3.forward;

        speed = tracerSpeed;
        traveled = 0f;

        transform.position = start;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    void Update()
    {
        float step = speed * Time.deltaTime;
        traveled += step;

        float t = (dist <= 0.0001f) ? 1f : Mathf.Clamp01(traveled / dist);
        transform.position = Vector3.Lerp(start, end, t);

        lifeTime -= Time.deltaTime;
        if (t >= 1f || lifeTime <= 0f)
            Destroy(gameObject);
    }
}
