using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public Transform muzzle;
    public Transform aimSource;

    [Header("Mobile Input")]
    [SerializeField] private MobileJoystick shootJoystick;

    [Header("Masks")]
    public LayerMask hitMask;

    [Header("Weapon")]
    [SerializeField] private PlayerWeaponInventory weaponInventory;

    [Header("SFX")]
    public AudioSource sfxSource;

    private WeaponData currentWeapon;

    private float nextFireTime;
    private int currentAmmo;
    private bool isReloading;

    public int CurrentAmmo => currentAmmo;

    public int MagazineSize
    {
        get
        {
            if (currentWeapon == null)
                return 0;

            if (WeaponUpgradeManager.Instance != null)
                return WeaponUpgradeManager.Instance.GetCurrentMagazineSize(currentWeapon);

            return currentWeapon.magazineSize;
        }
    }

    public bool IsReloading => isReloading;
    public WeaponData CurrentWeapon => currentWeapon;

    private void Awake()
    {
        if (aimSource == null)
            aimSource = transform;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (weaponInventory == null)
            weaponInventory = GetComponent<PlayerWeaponInventory>();

        ApplySFXVolume();
    }

    private void OnEnable()
    {
        if (weaponInventory != null)
            weaponInventory.OnWeaponChanged += HandleWeaponChanged;
    }

    private void OnDisable()
    {
        if (weaponInventory != null)
            weaponInventory.OnWeaponChanged -= HandleWeaponChanged;
    }

    private void Start()
    {
        if (weaponInventory != null && weaponInventory.CurrentWeapon != null)
            HandleWeaponChanged(weaponInventory.CurrentWeapon);
    }

    private void Update()
    {
        if (currentWeapon == null || isReloading)
            return;

        HandleShootingInput();
    }

    private void HandleShootingInput()
    {
        if (shootJoystick == null)
            return;

        if (!shootJoystick.IsPressed)
            return;

        if (currentWeapon.fireMode == WeaponFireMode.FullAuto)
        {
            TryShoot();
        }
        else if (currentWeapon.fireMode == WeaponFireMode.SemiAuto)
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            PlayEmptySFX();
            StartReload();
            return;
        }

        float finalFireRate = currentWeapon.fireRate;

        if (WeaponUpgradeManager.Instance != null)
            finalFireRate = WeaponUpgradeManager.Instance.GetCurrentFireRate(currentWeapon);

        if (finalFireRate <= 0f)
            finalFireRate = 1f;

        nextFireTime = Time.time + 1f / finalFireRate;

        Shoot();

        currentAmmo--;

        if (currentAmmo <= 0)
            StartReload();
    }

    private void Shoot()
    {
        if (muzzle == null || aimSource == null || currentWeapon == null)
            return;

        PlayShootSFX();
        PlayMuzzleFlash();

        if (currentWeapon.shootType == WeaponShootType.Shotgun)
        {
            ShootShotgun();
        }
        else
        {
            ShootSingleBullet(GetShootDirection());
        }
    }

    private void ShootSingleBullet(Vector3 shootDirection)
    {
        if (shootDirection.sqrMagnitude < 0.001f)
            return;

        Vector3 shootStart = muzzle.position;
        Vector3 shootEnd = shootStart + shootDirection * currentWeapon.range;

        Debug.DrawRay(shootStart, shootDirection * 5f, Color.red, 1f);

        if (Physics.Raycast(shootStart, shootDirection, out RaycastHit hit, currentWeapon.range, hitMask, QueryTriggerInteraction.Collide))
        {
            shootEnd = hit.point;

            ZombieAI zombie = hit.collider.GetComponent<ZombieAI>();

            if (zombie == null)
                zombie = hit.collider.GetComponentInParent<ZombieAI>();

            if (zombie != null)
            {
                int finalDamage = currentWeapon.damage;

                if (WeaponUpgradeManager.Instance != null)
                    finalDamage = WeaponUpgradeManager.Instance.GetCurrentDamage(currentWeapon);

                Debug.Log("[PlayerShooting] Damage dealt: " + finalDamage + " | Weapon: " + currentWeapon.weaponName);

                zombie.TakeDamage(finalDamage);
            }
        }

        SpawnTracer(shootStart, shootEnd);
    }

    private void ShootShotgun()
    {
        Vector3 baseDirection = GetShootDirection();

        for (int i = 0; i < currentWeapon.pellets; i++)
        {
            Vector3 spreadDirection = ApplySpread(baseDirection, currentWeapon.spreadAngle);
            ShootSingleBullet(spreadDirection);
        }
    }

    private Vector3 GetShootDirection()
    {
        Vector3 forward = aimSource.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = transform.forward;

        forward.Normalize();

        return forward;
    }

    private Vector3 ApplySpread(Vector3 direction, float angle)
    {
        float randomAngle = Random.Range(-angle, angle);
        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        return rotation * direction;
    }

    private void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (currentWeapon == null || currentWeapon.tracerPrefab == null)
            return;

        GameObject tracerObject = Instantiate(currentWeapon.tracerPrefab, start, Quaternion.identity);

        BulletTracer bulletTracer = tracerObject.GetComponent<BulletTracer>();

        if (bulletTracer != null)
        {
            bulletTracer.Setup(start, end, currentWeapon.tracerLifeTime);
            return;
        }

        Vector3 dir = end - start;
        float distance = dir.magnitude;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Vector3 forward = dir.normalized;

            tracerObject.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

            Vector3 scale = tracerObject.transform.localScale;
            scale.z = distance;
            tracerObject.transform.localScale = scale;

            tracerObject.transform.position = start + forward * (distance * 0.5f);
        }

        Destroy(tracerObject, currentWeapon.tracerLifeTime);
    }

    private void PlayMuzzleFlash()
    {
        if (currentWeapon == null || currentWeapon.muzzleFlashPrefab == null || muzzle == null)
            return;

        ParticleSystem flash = Instantiate(
            currentWeapon.muzzleFlashPrefab,
            muzzle.position,
            muzzle.rotation
        );

        Destroy(flash.gameObject, 1f);
    }

    private void PlayShootSFX()
    {
        if (sfxSource == null || currentWeapon == null || currentWeapon.shootClip == null)
            return;

        ApplySFXVolume();
        sfxSource.PlayOneShot(currentWeapon.shootClip);
    }

    private void PlayReloadSFX()
    {
        if (sfxSource == null || currentWeapon == null || currentWeapon.reloadClip == null)
            return;

        ApplySFXVolume();
        sfxSource.PlayOneShot(currentWeapon.reloadClip);
    }

    private void PlayEmptySFX()
    {
        if (sfxSource == null || currentWeapon == null || currentWeapon.emptyClip == null)
            return;

        ApplySFXVolume();
        sfxSource.PlayOneShot(currentWeapon.emptyClip);
    }

    private void ApplySFXVolume()
    {
        if (sfxSource == null)
            return;

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSource.volume = sfxVolume;
    }

    private void HandleWeaponChanged(WeaponData weapon)
    {
        currentWeapon = weapon;
        isReloading = false;

        currentAmmo = currentWeapon != null ? MagazineSize : 0;

        Debug.Log(
            "[PlayerShooting] Weapon changed: " +
            (currentWeapon != null ? currentWeapon.weaponName : "NULL") +
            " | Ammo: " + currentAmmo +
            " / " + MagazineSize
        );
    }

    public void StartReload()
    {
        if (currentWeapon == null)
            return;

        if (isReloading)
            return;

        if (currentAmmo >= MagazineSize)
            return;

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        PlayReloadSFX();

        Debug.Log("[PlayerShooting] Reload started");

        float finalReloadTime = currentWeapon.reloadTime;

        if (WeaponUpgradeManager.Instance != null)
            finalReloadTime = WeaponUpgradeManager.Instance.GetCurrentReloadTime(currentWeapon);

        if (finalReloadTime < 0.05f)
            finalReloadTime = 0.05f;

        yield return new WaitForSeconds(finalReloadTime);

        currentAmmo = MagazineSize;
        isReloading = false;

        Debug.Log("[PlayerShooting] Reload completed. Ammo: " + currentAmmo + " / " + MagazineSize);
    }
}