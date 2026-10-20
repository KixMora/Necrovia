using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerWeaponInventory weaponInventory;
    [SerializeField] private PlayerShooting playerShooting;

    [Header("Weapon Holder")]
    [SerializeField] private Transform weaponHolder;

    private GameObject currentWeaponModel;

    private void Awake()
    {
        if (weaponInventory == null)
            weaponInventory = GetComponent<PlayerWeaponInventory>();

        if (playerShooting == null)
            playerShooting = GetComponent<PlayerShooting>();
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
        {
            HandleWeaponChanged(weaponInventory.CurrentWeapon);
        }
    }

    private void HandleWeaponChanged(WeaponData weapon)
    {
        ClearCurrentWeapon();

        if (weapon == null)
        {
            Debug.LogWarning("[WeaponVisualController] Weapon is NULL");
            return;
        }

        if (weapon.weaponModelPrefab == null)
        {
            Debug.LogWarning("[WeaponVisualController] Weapon model prefab is NULL for: " + weapon.weaponName);
            return;
        }

        if (weaponHolder == null)
        {
            Debug.LogError("[WeaponVisualController] WeaponHolder is NULL");
            return;
        }

        currentWeaponModel = Instantiate(weapon.weaponModelPrefab, weaponHolder);

        currentWeaponModel.transform.localPosition = Vector3.zero;
        currentWeaponModel.transform.localRotation = Quaternion.identity;
        currentWeaponModel.transform.localScale = Vector3.one;

        AssignMuzzle(currentWeaponModel);

        Debug.Log("[WeaponVisualController] Spawned weapon model: " + weapon.weaponName);
    }

    private void ClearCurrentWeapon()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
            currentWeaponModel = null;
        }
    }

    private void AssignMuzzle(GameObject weaponModel)
    {
        if (playerShooting == null)
        {
            Debug.LogWarning("[WeaponVisualController] PlayerShooting is NULL");
            return;
        }

        Transform muzzle = FindDeepChild(weaponModel.transform, "Muzzle");

        if (muzzle == null)
        {
            Debug.LogWarning("[WeaponVisualController] Muzzle not found in weapon prefab: " + weaponModel.name);
            return;
        }

        playerShooting.muzzle = muzzle;

        Debug.Log("[WeaponVisualController] Muzzle assigned: " + muzzle.name);
    }

    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = FindDeepChild(child, childName);

            if (result != null)
                return result;
        }

        return null;
    }
}