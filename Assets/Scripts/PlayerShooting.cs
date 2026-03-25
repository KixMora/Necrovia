using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public Transform muzzle;
    [Header("VFX")]
    public ParticleSystem muzzleFlashPrefab;

    [Header("Shoot")]
    public float fireRate = 10f;
    public float range = 50f;
    public LayerMask hitMask;

    [Header("Tracer Bullet")]
    public TracerMover tracerPrefab;
    public float tracerSpeed = 80f;

    private float nextFireTime;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            TryShoot();
            //PlayMuzzleFlash();
        }
          
    }
    void PlayMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzle == null) return;

        ParticleSystem ps = Instantiate(muzzleFlashPrefab, muzzle.position, muzzle.rotation, muzzle);
        ps.Play();

        Destroy(ps.gameObject, 0.2f); // на всякий случай
    }

    void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + (1f / fireRate);

        Vector3 dir = transform.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        dir.Normalize();

        // старт немного впереди, чтобы не попадать в себя
        Vector3 origin = (muzzle != null ? muzzle.position : transform.position + Vector3.up) + dir * 0.2f;

        Vector3 endPoint = origin + dir * range;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            endPoint = hit.point;

            ZombieAI zombie = hit.collider.GetComponentInParent<ZombieAI>();
            if (zombie != null)
            {
                zombie.TakeDamage(10f);
            }
        }


        // фиксированная высота трассера (стабильно в top-down)
        float y = origin.y;
        origin.y = y;
        endPoint.y = y;

        SpawnTracer(origin, endPoint);
    }

    void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (tracerPrefab == null) return;

        TracerMover t = Instantiate(tracerPrefab);
        t.Init(start, end, tracerSpeed);
    }
}
