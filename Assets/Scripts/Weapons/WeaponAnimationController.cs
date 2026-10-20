using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerWeaponInventory weaponInventory;

    [Header("Base Controller")]
    [SerializeField] private RuntimeAnimatorController baseController;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (weaponInventory == null)
            weaponInventory = GetComponent<PlayerWeaponInventory>();

        if (animator != null && baseController == null)
            baseController = animator.runtimeAnimatorController;
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

    private void HandleWeaponChanged(WeaponData weapon)
    {
        if (animator == null)
        {
            Debug.LogError("[WeaponAnimationController] Animator is NULL");
            return;
        }

        if (weapon == null)
        {
            Debug.LogWarning("[WeaponAnimationController] Weapon is NULL");
            return;
        }

        if (weapon.animatorOverride != null)
        {
            animator.runtimeAnimatorController = weapon.animatorOverride;
            Debug.Log("[WeaponAnimationController] Applied animator override: " + weapon.weaponName);
        }
        else
        {
            animator.runtimeAnimatorController = baseController;
            Debug.Log("[WeaponAnimationController] Applied base animator controller");
        }
    }
}